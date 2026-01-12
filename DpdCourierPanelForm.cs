using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
// Upewnij się, że referencje do usług są dodane i mają dokładnie takie przestrzenie nazw
using Reklamacje_Dane.DpdPackageService;
using Reklamacje_Dane.DpdInfoService;

namespace Reklamacje_Dane
{
    public partial class DpdCourierPanelForm : Form
    {
        private long? currentSessionId = null;
        private long? currentPackageId = null;
        private int? zgłoszenieId = null;
        private string lastGeneratedWaybill = null;

        private string dpdLogin;
        private string dpdPassword;
        private int dpdMasterFid; // Poprawiono typ na int

        private DpdDatabaseService dbService = new DpdDatabaseService();

        public DpdCourierPanelForm(int? idZgłoszenia = null)
        {
            InitializeComponent();
            this.zgłoszenieId = idZgłoszenia;
            if (this.zgłoszenieId.HasValue)
            {
                LogToJournal($"Formularz otwarty w kontekście zgłoszenia ID: {this.zgłoszenieId.Value}");
            }
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void DpdCourierPanelForm_Load(object sender, EventArgs e)
        {
            LoadSettings();
            ResetFormState();
        }

        #region Logika Głównego Procesu

        private async void BtnCreateShipment_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
            {
                MessageBox.Show("Proszę wypełnić wszystkie wymagane pola.", "Błąd Walidacji", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ResetFormState();
            LogToJournal("Rozpoczęcie tworzenia przesyłki...");
            SetStatus("Tworzenie przesyłki w systemie DPD...", true);

            try
            {
                // Poprawne tworzenie obiektu usług
                var packageServices = new servicesOpenUMLFeV11();
                if (checkBoxDeclaredValue.Checked && decimal.TryParse(textBoxDeclaredValueAmount.Text, out decimal declaredAmount))
                {
                    packageServices.declaredValue = new serviceDeclaredValueOpenUMLFeV1
                    {
                        // Poprawka: API oczekuje string i enum
                        amount = declaredAmount.ToString(CultureInfo.InvariantCulture),
                        currency = serviceCurrencyEnum.PLN,
                        currencySpecified = true
                    };
                }
                if (checkBoxCOD.Checked && decimal.TryParse(textBoxCODAmount.Text, out decimal codAmount))
                {
                    packageServices.cod = new serviceCodOpenUMLFeV1
                    {
                        // Poprawka: API oczekuje string i enum
                        amount = codAmount.ToString(CultureInfo.InvariantCulture),
                        currency = serviceCurrencyEnum.PLN,
                        currencySpecified = true
                    };
                }
                if (checkBoxROD.Checked)
                {
                    packageServices.rod = new serviceRodOpenUMLFeV1();
                }

                var package = new packageOpenUMLFeV11
                {
                    payerType = payerTypeEnumOpenUMLFeV1.THIRD_PARTY,
                    payerTypeSpecified = true,
                    thirdPartyFID = dpdMasterFid,
                    thirdPartyFIDSpecified = true,
                    sender = new packageAddressOpenUMLFeV1
                    {
                        name = textBoxSenderName.Text,
                        address = textBoxSenderAddress.Text,
                        city = textBoxSenderCity.Text,
                        postalCode = textBoxSenderPostalCode.Text.Replace("-", ""),
                        countryCode = "PL",
                        phone = textBoxSenderPhone.Text
                    },
                    receiver = new packageAddressOpenUMLFeV1
                    {
                        name = textBoxReceiverName.Text,
                        address = textBoxReceiverAddress.Text,
                        city = textBoxReceiverCity.Text,
                        postalCode = textBoxReceiverPostalCode.Text.Replace("-", ""),
                        countryCode = "PL",
                        phone = textBoxReceiverPhone.Text
                    },
                    parcels = new[] {
                        new parcelOpenUMLFeV11
                        {
                            weight = (double)numericUpDownWeight.Value,
                            weightSpecified = true,
                            sizeX = (int)numericUpDownSizeX.Value,
                            sizeXSpecified = true,
                            sizeY = (int)numericUpDownSizeY.Value,
                            sizeYSpecified = true,
                            sizeZ = (int)numericUpDownSizeZ.Value,
                            sizeZSpecified = true,
                            content = textBoxContent.Text
                        }
                    },
                    services = packageServices // Poprawione przypisanie obiektu usług
                };

                var client = new DPDPackageObjServicesClient();
                var response = await Task.Run(() => client.generatePackagesNumbersV9(
                    new openUMLFeV11 { packages = new[] { package } },
                    pkgNumsGenerationPolicyV1.ALL_OR_NOTHING,
                    "PL",
                    GetAuthData()));

                LogToJournal($"Odpowiedź z generatePackagesNumbersV9: Status={response.Status}");

                if (response.Status == "OK" && response.Packages.Any())
                {
                    var resultPackage = response.Packages.First();
                    if (resultPackage.Status == "OK" && resultPackage.Parcels.Any())
                    {
                        var resultParcel = resultPackage.Parcels.First();
                        currentSessionId = response.SessionId;
                        currentPackageId = resultPackage.PackageId;
                        lastGeneratedWaybill = resultParcel.Waybill; // Zapisz numer listu

                        LogToJournal($"Sukces! Utworzono przesyłkę. Waybill: {resultParcel.Waybill}, PackageID: {currentPackageId}, SessionID: {currentSessionId}");
                        MessageBox.Show($"Przesyłka została pomyślnie utworzona!\nNumer listu przewozowego: {resultParcel.Waybill}", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        btnOrderCourier.Enabled = true;
                        btnGenerateLabel.Enabled = true;
                        btnGenerateProtocol.Enabled = true;
                        btnCreateShipment.Enabled = false;
                        SetStatus("Gotowy do zamówienia kuriera lub generowania dokumentów.");
                    }
                    else
                    {
                        var errorInfo = resultPackage.ValidationDetails?.FirstOrDefault()?.Info ?? "Nieznany błąd w przesyłce.";
                        throw new Exception(errorInfo);
                    }
                }
                else
                {
                    var errorInfo = response.Packages?.FirstOrDefault()?.ValidationDetails?.FirstOrDefault()?.Info ?? response.Status;
                    throw new Exception(errorInfo);
                }
            }
            catch (Exception ex)
            {
                LogToJournal($"BŁĄD przy tworzeniu przesyłki: {ex.Message}");
                MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Błąd API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Błąd podczas tworzenia przesyłki.", false);
            }
        }

        private async void BtnOrderCourier_Click(object sender, EventArgs e)
        {
            if (comboBoxPickupTime.SelectedItem == null)
            {
                MessageBox.Show("Proszę wybrać godzinę odbioru.", "Błąd Walidacji", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LogToJournal("Rozpoczęcie zamawiania kuriera...");
            SetStatus("Zamawianie kuriera...", true);

            try
            {
                var pickupTimeRange = comboBoxPickupTime.SelectedItem.ToString().Split('-');
                var dpdParams = new dpdPickupCallParamsV3
                {
                    operationType = pickupCallOperationTypeDPPEnumV1.INSERT,
                    // Poprawka: konwersja daty na string
                    pickupDate = dateTimePickerPickup.Value.ToString("yyyy-MM-dd"),
                    pickupTimeFrom = pickupTimeRange[0].Trim(),
                    pickupTimeTo = pickupTimeRange[1].Trim(),
                    pickupCallSimplifiedDetails = new pickupCallSimplifiedDetailsDPPV1
                    {
                        pickupPayer = new pickupPayerDPPV1
                        {
                            payerNumber = dpdMasterFid,
                            payerNumberSpecified = true
                        },
                        pickupSender = new pickupSenderDPPV1
                        {
                            senderFullName = textBoxSenderName.Text,
                            senderAddress = textBoxSenderAddress.Text,
                            senderCity = textBoxSenderCity.Text,
                            senderPostalCode = textBoxSenderPostalCode.Text,
                            senderPhone = textBoxSenderPhone.Text
                        },
                        packagesParams = new pickupPackagesParamsDPPV1
                        {
                            parcelsCount = 1,
                            parcelsCountSpecified = true,
                            parcelsWeight = (double)numericUpDownWeight.Value,
                            parcelsWeightSpecified = true,
                            standardParcel = true,
                            standardParcelSpecified = true
                        }
                    }
                };

                var client = new DPDPackageObjServicesClient();
                var response = await Task.Run(() => client.packagesPickupCallV4(dpdParams, GetAuthData()));

                LogToJournal($"Odpowiedź z packagesPickupCallV4: Status={response.StatusInfo.Status}");

                if (response.StatusInfo.Status == "OK")
                {
                    LogToJournal($"Sukces! Zamówiono kuriera. Numer zlecenia: {response.OrderNumber}");
                    MessageBox.Show($"Kurier został pomyślnie zamówiony!\nNumer zlecenia: {response.OrderNumber}", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SetStatus("Kurier zamówiony. Przesyłka gotowa do nadania.");
                    btnOrderCourier.Enabled = false;
                }
                else
                {
                    var errorInfo = response.StatusInfo.ErrorDetails?.FirstOrDefault()?.Description ?? "Nieznany błąd.";
                    throw new Exception(errorInfo);
                }
            }
            catch (Exception ex)
            {
                LogToJournal($"BŁĄD przy zamawianiu kuriera: {ex.Message}");
                MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Błąd API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Błąd podczas zamawiania kuriera.", false);
            }
        }

        #endregion

        #region Walidacja, UI i Funkcje Pomocnicze

        private bool ValidateForm()
        {
            return !string.IsNullOrWhiteSpace(textBoxSenderName.Text) &&
                   !string.IsNullOrWhiteSpace(textBoxSenderAddress.Text) &&
                   !string.IsNullOrWhiteSpace(textBoxSenderCity.Text) &&
                   !string.IsNullOrWhiteSpace(textBoxSenderPostalCode.Text) &&
                   !string.IsNullOrWhiteSpace(textBoxSenderPhone.Text) &&
                   !string.IsNullOrWhiteSpace(textBoxReceiverName.Text) &&
                   !string.IsNullOrWhiteSpace(textBoxReceiverAddress.Text) &&
                   !string.IsNullOrWhiteSpace(textBoxReceiverCity.Text) &&
                   !string.IsNullOrWhiteSpace(textBoxReceiverPostalCode.Text) &&
                   !string.IsNullOrWhiteSpace(textBoxReceiverPhone.Text) &&
                   !string.IsNullOrWhiteSpace(textBoxContent.Text);
        }

        private void ResetFormState()
        {
            currentSessionId = null;
            currentPackageId = null;
            lastGeneratedWaybill = null;
            btnCreateShipment.Enabled = true;
            btnOrderCourier.Enabled = false;
            btnGenerateLabel.Enabled = false;
            btnGenerateProtocol.Enabled = false;
            SetStatus("Gotowy.");
        }

        // Poprawka: Użycie pełnej nazwy typu, aby rozwiązać niejednoznaczność
        private Reklamacje_Dane.DpdPackageService.authDataV1 GetAuthData()
        {
            return new Reklamacje_Dane.DpdPackageService.authDataV1
            {
                login = this.dpdLogin,
                password = this.dpdPassword,
                masterFid = this.dpdMasterFid, // typ int jest już zgodny
                masterFidSpecified = true
            };
        }

        private void CheckBoxDeclaredValue_CheckedChanged(object sender, EventArgs e)
        {
            textBoxDeclaredValueAmount.Enabled = checkBoxDeclaredValue.Checked;
        }

        private void CheckBoxCOD_CheckedChanged(object sender, EventArgs e)
        {
            textBoxCODAmount.Enabled = checkBoxCOD.Checked;
        }

        private async void BtnValidateSenderPostalCode_Click(object sender, EventArgs e)
        {
            await ValidatePostalCode(textBoxSenderPostalCode);
        }

        private async void BtnValidateReceiverPostalCode_Click(object sender, EventArgs e)
        {
            await ValidatePostalCode(textBoxReceiverPostalCode);
        }

        private async void TextBoxSenderPostalCode_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBoxSenderPostalCode.Text))
            {
                await GetCourierAvailability(textBoxSenderPostalCode.Text);
            }
        }

        private async Task ValidatePostalCode(TextBox postalCodeTextBox)
        {
            if (string.IsNullOrWhiteSpace(postalCodeTextBox.Text)) return;
            SetStatus($"Walidacja kodu {postalCodeTextBox.Text}...", true);
            try
            {
                var client = new DPDPackageObjServicesClient();
                var response = await Task.Run(() => client.findPostalCodeV1(new postalCodeV1 { countryCode = "PL", zipCode = postalCodeTextBox.Text.Replace("-", "") }, GetAuthData()));

                if (response.status == "OK")
                {
                    postalCodeTextBox.BackColor = Color.LightGreen;
                    SetStatus($"Kod pocztowy {postalCodeTextBox.Text} jest poprawny.");
                }
                else
                {
                    postalCodeTextBox.BackColor = Color.Salmon;
                    SetStatus($"Kod pocztowy {postalCodeTextBox.Text} jest niepoprawny.");
                }
            }
            catch (Exception ex)
            {
                LogToJournal($"Błąd walidacji kodu pocztowego: {ex.Message}");
                SetStatus("Błąd walidacji kodu pocztowego.", false);
            }
        }

        private async Task GetCourierAvailability(string postalCode)
        {
            SetStatus($"Sprawdzanie dostępności kuriera dla {postalCode}...", true);
            try
            {
                var client = new DPDPackageObjServicesClient();
                var response = await Task.Run(() => client.getCourierOrderAvailabilityV1(new senderPlaceV1 { countryCode = "PL", zipCode = postalCode.Replace("-", "") }, GetAuthData()));

                comboBoxPickupTime.Items.Clear();
                if (response.status == "OK" && response.ranges != null)
                {
                    foreach (var range in response.ranges)
                    {
                        comboBoxPickupTime.Items.Add(range.range);
                    }
                    if (comboBoxPickupTime.Items.Count > 0) comboBoxPickupTime.SelectedIndex = 0;
                    SetStatus("Dostępne terminy odbioru zostały zaktualizowane.");
                }
                else
                {
                    SetStatus("Nie znaleziono dostępnych terminów odbioru.", false);
                }
            }
            catch (Exception ex)
            {
                LogToJournal($"Błąd sprawdzania dostępności kuriera: {ex.Message}");
                SetStatus("Błąd sprawdzania dostępności kuriera.", false);
            }
        }

        private async void BtnGenerateLabel_Click(object sender, EventArgs e)
        {
            if (!currentSessionId.HasValue) return;

            SetStatus("Generowanie etykiety...", true);
            try
            {
                var client = new DPDPackageObjServicesClient();
                var requestParams = new dpdServicesParamsV1
                {
                    // Poprawka: Użycie poprawnych nazw właściwości
                    session = new sessionDSPV2 { SessionId = currentSessionId.Value, SessionIdSpecified = true, SessionType = sessionTypeDSPEnum.DOMESTIC, SessionTypeSpecified = true }
                };

                var response = await Task.Run(() => client.generateSpedLabelsV4(requestParams, outputDocFormatDSPEnumV1.PDF, outputDocPageFormatDSPEnumV1.LBL_PRINTER, outputLabelTypeEnumV1.BIC3, null, GetAuthData()));

                if (response.Session.StatusInfo.Status == "OK")
                {
                    string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Etykieta_{lastGeneratedWaybill}.pdf");
                    File.WriteAllBytes(filePath, response.documentData);
                    System.Diagnostics.Process.Start(filePath);
                }
                else
                {
                    throw new Exception(response.Session.StatusInfo.Description ?? "Nieznany błąd generowania etykiety.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Błąd API", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetStatus("Gotowy.");
            }
        }

        private async void BtnGenerateProtocol_Click(object sender, EventArgs e)
        {
            if (!currentSessionId.HasValue) return;

            SetStatus("Generowanie protokołu...", true);
            try
            {
                var client = new DPDPackageObjServicesClient();
                var requestParams = new dpdServicesParamsV1
                {
                    // Poprawka: Użycie poprawnych nazw właściwości
                    session = new sessionDSPV2 { SessionId = currentSessionId.Value, SessionIdSpecified = true, SessionType = sessionTypeDSPEnum.DOMESTIC, SessionTypeSpecified = true }
                };

                var response = await Task.Run(() => client.generateProtocolV2(requestParams, outputDocFormatDSPEnumV1.PDF, outputDocPageFormatDSPEnumV1.A4, GetAuthData()));

                if (response.session.StatusInfo.Status == "OK")
                {
                    string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Protokol_{currentPackageId}.pdf");
                    File.WriteAllBytes(filePath, response.documentData);
                    System.Diagnostics.Process.Start(filePath);
                }
                else
                {
                    throw new Exception(response.session.StatusInfo.Description ?? "Nieznany błąd generowania protokołu.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Błąd API", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetStatus("Gotowy.");
            }
        }

        private async void BtnTrackPackage_Click(object sender, EventArgs e)
        {
            string waybill = textBoxWaybillTrack.Text;
            if (string.IsNullOrWhiteSpace(waybill))
            {
                MessageBox.Show("Proszę podać numer listu przewozowego.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetStatus($"Śledzenie paczki {waybill}...", true);
            try
            {
                var client = new DPDInfoServicesObjEventsClient();
                // Poprawka: Użycie pełnej nazwy typu, aby rozwiązać niejednoznaczność
                var authData = new Reklamacje_Dane.DpdInfoService.authDataV1 { login = this.dpdLogin, password = this.dpdPassword, channel = this.dpdMasterFid.ToString() };

                var response = await Task.Run(() => client.getEventsForWaybillV1(waybill, eventsSelectTypeEnum.ALL, "PL", authData));

                var events = response.eventsList;
                dataGridViewEvents.DataSource = events?.Select(ev => new {
                    Data = ev.eventTime,
                    Opis = ev.description,
                    Oddział = ev.depotName
                }).ToList();

                if (events == null || !events.Any())
                {
                    MessageBox.Show("Nie znaleziono zdarzeń dla podanego numeru listu.", "Brak Danych", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SetStatus("Nie znaleziono zdarzeń.");
                    return;
                }

                var returnEvent = events.FirstOrDefault(ev => ev.businessCode == "230403" || ev.businessCode == "230408" || ev.businessCode == "230402");
                if (returnEvent != null)
                {
                    var newWaybillData = returnEvent.eventDataList?.FirstOrDefault(ed => !string.IsNullOrEmpty(ed.value));
                    if (newWaybillData != null)
                    {
                        string newWaybill = newWaybillData.value;
                        if (MessageBox.Show($"Wykryto, że paczka została zwrócona lub przekierowana.\nNowy numer listu przewozowego to: {newWaybill}\n\nCzy chcesz teraz śledzić nową paczkę?", "Wykryto Nową Paczkę", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            textBoxWaybillTrack.Text = newWaybill;
                            BtnTrackPackage_Click(sender, e);
                            return;
                        }
                    }
                }
                SetStatus($"Zakończono śledzenie paczki {waybill}.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Błąd API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Błąd podczas śledzenia paczki.", false);
            }
        }

        #endregion

        #region Logowanie i Ustawienia

        private void LogToJournal(string message)
        {
            string logEntry = $"{DateTime.Now:G}: {message}{Environment.NewLine}";
            if (textBoxJournal.InvokeRequired)
            {
                textBoxJournal.Invoke(new Action(() => textBoxJournal.AppendText(logEntry)));
            }
            else
            {
                textBoxJournal.AppendText(logEntry);
            }
            dbService.LogToJournal(logEntry);

            if (zgłoszenieId.HasValue)
            {
                dbService.LogToActionHistory(zgłoszenieId.Value, message);
            }
        }

        private void SetStatus(string message, bool inProgress = false)
        {
            Action action = () => {
                toolStripStatusLabel.Text = message;
                statusStrip1.BackColor = inProgress ? Color.LightYellow : SystemColors.Control;
                this.Cursor = inProgress ? Cursors.WaitCursor : Cursors.Default;
            };

            if (statusStrip1.InvokeRequired)
            {
                statusStrip1.Invoke(action);
            }
            else
            {
                action();
            }
        }

        private void LoadSettings()
        {
            this.dpdLogin = "test";
            this.dpdPassword = "thetu4Ee";
            this.dpdMasterFid = 1495; // Poprawiono typ na int

            textBoxSenderName.Text = "Firma Testowa Sp. z o.o.";
            textBoxSenderAddress.Text = "ul. Testowa 1";
            textBoxSenderCity.Text = "Warszawa";
            textBoxSenderPostalCode.Text = "02-274";
            textBoxSenderPhone.Text = "500600700";

            LogToJournal("Ustawienia i dane domyślne załadowane.");
        }

        /// <summary>
        /// Włącza sprawdzanie pisowni po polsku dla wszystkich TextBoxów w formularzu
        /// </summary>
        private void EnableSpellCheckOnAllTextBoxes()
        {
            try
            {
                foreach (Control control in GetAllControls(this))
                {
                    if (control is RichTextBox richTextBox)
                    {
                        richTextBox.EnableSpellCheck(true);
                    }
                    else if (control is TextBox textBox && !(textBox is SpellCheckTextBox))
                    {
                        textBox.EnableSpellCheck(false);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd włączania sprawdzania pisowni: {ex.Message}");
            }
        }

        /// <summary>
        /// Rekurencyjnie pobiera wszystkie kontrolki z kontenera
        /// </summary>
        private IEnumerable<Control> GetAllControls(Control container)
        {
            foreach (Control control in container.Controls)
            {
                yield return control;

                if (control.HasChildren)
                {
                    foreach (Control child in GetAllControls(control))
                    {
                        yield return child;
                    }
                }
            }
        }

        #endregion
    }

    // Usunięto zduplikowaną klasę DpdDatabaseService.
    // Upewnij się, że masz ją zdefiniowaną w osobnym pliku w swoim projekcie.
}
