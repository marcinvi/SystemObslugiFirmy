using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace Reklamacje_Dane
{
    public partial class FormUploader : Form
    {
        private readonly string _targetPath;
        // Lista obsługiwanych rozszerzeń dla podglądu
        private static readonly string[] ImageExtensions = { ".JPG", ".JPEG", ".JPE", ".BMP", ".GIF", ".PNG" };

        // KONSTRUKTOR NAPRAWIAJĄCY BŁĄD CS1729
        // Przyjmuje opcjonalny PhoneClient, ale korzysta z Singletona
        public FormUploader(string nrZgloszenia, PhoneClient pc = null)
        {
            InitializeComponent();
            this.TopMost = true; // Zawsze na wierzchu

            // Tworzenie ścieżki zapisu
            string folderName = nrZgloszenia.Replace('/', '.');
            _targetPath = Path.Combine(AppPaths.GetDataRootPath(), folderName);
            labelTitle.Text = $"Dodaj załączniki do: {nrZgloszenia}";
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        // ==========================================================
        // SEKCJA 1: POBIERANIE Z TELEFONU (Z wykrywaniem rozszerzeń)
        // ==========================================================
        private async void btnFromPhone_Click(object sender, EventArgs e)
        {
            if (PhoneClient.Instance == null)
            {
                MessageBox.Show("Brak połączenia z telefonem!");
                return;
            }

            using (var gallery = new FormPhoneGallery(PhoneClient.Instance))
            {
                if (gallery.ShowDialog(this) == DialogResult.OK)
                {
                    await ImportFromPhone(gallery.SelectedPaths);
                }
            }
        }

        private async Task ImportFromPhone(List<string> remoteUrls)
        {
            SetUIState(false);
            progressBarCopy.Visible = true;
            progressBarCopy.Value = 0;
            progressBarCopy.Maximum = remoteUrls.Count;

            try
            {
                Directory.CreateDirectory(_targetPath);
                for (int i = 0; i < remoteUrls.Count; i++)
                {
                    string url = remoteUrls[i];

                    // Pobieramy pełny plik (false = oryginał)
                    byte[] data = await PhoneClient.Instance.DownloadPhotoAsync(url, false);

                    if (data != null && data.Length > 0)
                    {
                        // AUTOMATYCZNE WYKRYWANIE FORMATU (Magic Bytes)
                        string ext = ".jpg"; // Domyślny
                        if (data.Length > 10)
                        {
                            // MP4 (ftyp)
                            if (data[4] == 'f' && data[5] == 't' && data[6] == 'y' && data[7] == 'p') ext = ".mp4";
                            // PNG
                            else if (data[0] == 0x89 && data[1] == 0x50) ext = ".png";
                            // GIF
                            else if (data[0] == 0x47 && data[1] == 0x49) ext = ".gif";
                        }

                        string fileName = $"TEL_{DateTime.Now:yyyyMMdd_HHmmss}_{i}{ext}";
                        string destPath = Path.Combine(_targetPath, fileName);

                        await Task.Run(() => File.WriteAllBytes(destPath, data));
                        listBoxFiles.Items.Add($"Pobrano: {fileName}");
                    }
                    progressBarCopy.Value = i + 1;
                }
            }
            catch (Exception ex) { ShowCriticalError(ex); }
            finally { ResetUIState(); }
        }

        // ==========================================================
        // SEKCJA 2: OBSŁUGA PLIKÓW LOKALNYCH (Browse)
        // ==========================================================
        private async void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Multiselect = true, Title = "Wybierz pliki" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    await HandleDroppedFiles(ofd.FileNames);
                }
            }
        }

        // ==========================================================
        // SEKCJA 3: DRAG & DROP (Kompletna implementacja)
        // ==========================================================
        private void panelDropZone_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent("FileGroupDescriptorW"))
            {
                e.Effect = DragDropEffects.Copy;
                panelDropZone.BackColor = Color.LightSkyBlue;
            }
            else e.Effect = DragDropEffects.None;
        }

        private async void panelDropZone_DragDrop(object sender, DragEventArgs e)
        {
            panelDropZone.BackColor = Color.WhiteSmoke;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                await HandleDroppedFiles((string[])e.Data.GetData(DataFormats.FileDrop));
            else if (e.Data.GetDataPresent("FileGroupDescriptorW"))
                await HandleVirtualFiles(e.Data);
        }

        private void panelDropZone_DragLeave(object sender, EventArgs e)
        {
            panelDropZone.BackColor = Color.WhiteSmoke;
        }

        private async Task HandleDroppedFiles(string[] filePaths)
        {
            SetUIState(false);
            progressBarCopy.Visible = true;
            progressBarCopy.Value = 0;
            progressBarCopy.Maximum = filePaths.Length;
            try
            {
                Directory.CreateDirectory(_targetPath);
                int count = 0;
                foreach (string sourcePath in filePaths)
                {
                    string originalFileName = Path.GetFileName(sourcePath);
                    string destPath = Path.Combine(_targetPath, originalFileName);

                    if (File.Exists(destPath))
                    {
                        destPath = await ResolveFileNameConflict(originalFileName);
                        if (destPath == null) { count++; progressBarCopy.Value = count; continue; }
                    }

                    await Task.Run(() => File.Copy(sourcePath, destPath, true));
                    listBoxFiles.Items.Add($"Skopiowano: {Path.GetFileName(destPath)}");
                    count++;
                    progressBarCopy.Value = count;
                }
            }
            catch (Exception ex) { ShowCriticalError(ex); }
            finally { ResetUIState(); }
        }

        // Obsługa plików przeciąganych z przeglądarki (np. Chrome)
        private async Task HandleVirtualFiles(IDataObject dataObject)
        {
            var fileDescriptorStream = (MemoryStream)dataObject.GetData("FileGroupDescriptorW");
            if (fileDescriptorStream == null) return;

            var fileNames = GetVirtualFileNames(fileDescriptorStream);
            MemoryStream[] fileContentsArray = null;
            object fileContentsObject = dataObject.GetData("FileContents");

            if (fileContentsObject is MemoryStream[] streams) fileContentsArray = streams;
            else if (fileContentsObject is MemoryStream singleStream) fileContentsArray = new MemoryStream[] { singleStream };

            if (fileContentsArray == null) { MessageBox.Show("Błąd odczytu z przeglądarki."); return; }

            SetUIState(false);
            progressBarCopy.Visible = true;
            progressBarCopy.Value = 0;
            progressBarCopy.Maximum = fileNames.Count;

            try
            {
                Directory.CreateDirectory(_targetPath);
                for (int i = 0; i < fileNames.Count; i++)
                {
                    if (i >= fileContentsArray.Length) continue;
                    string fileName = fileNames[i];
                    string destPath = Path.Combine(_targetPath, fileName);

                    if (File.Exists(destPath))
                    {
                        destPath = await ResolveFileNameConflict(fileName);
                        if (destPath == null) { progressBarCopy.Value = i + 1; continue; }
                    }

                    var content = fileContentsArray[i];
                    if (content != null)
                    {
                        byte[] bytes = content.ToArray();
                        await Task.Run(() => File.WriteAllBytes(destPath, bytes));
                        listBoxFiles.Items.Add($"Skopiowano: {Path.GetFileName(destPath)}");
                    }
                    progressBarCopy.Value = i + 1;
                }
            }
            catch (Exception ex) { ShowCriticalError(ex); }
            finally { ResetUIState(); }
        }

        // ==========================================================
        // SEKCJA 4: METODY POMOCNICZE
        // ==========================================================
        private List<string> GetVirtualFileNames(MemoryStream descriptorStream)
        {
            var fileNames = new List<string>();
            byte[] descriptorBytes = descriptorStream.ToArray();
            int fileCount = BitConverter.ToInt32(descriptorBytes, 0);
            using (var reader = new BinaryReader(descriptorStream))
            {
                for (int i = 0; i < fileCount; i++)
                {
                    reader.BaseStream.Seek(4 + (i * 592) + 72, SeekOrigin.Begin);
                    byte[] nameBytes = reader.ReadBytes(520);
                    string rawName = Encoding.Unicode.GetString(nameBytes).TrimEnd('\0');
                    fileNames.Add(SanitizeFileName(rawName));
                }
            }
            return fileNames;
        }

        private string SanitizeFileName(string fileName)
        {
            string name = Path.GetFileNameWithoutExtension(fileName);
            string ext = Path.GetExtension(fileName);
            foreach (char c in Path.GetInvalidFileNameChars()) name = name.Replace(c, '_');
            return name + ext;
        }

        private async Task<string> ResolveFileNameConflict(string originalFileName)
        {
            using (var inputBox = new FormInputBox("Plik istnieje. Podaj nową nazwę:", originalFileName))
            {
                if (inputBox.ShowDialog(this) == DialogResult.OK)
                    return Path.Combine(_targetPath, inputBox.NewFileName);
                return null;
            }
        }

        private void listBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBoxPreview.Image = null;
            if (listBoxFiles.SelectedItem == null) return;
            string txt = listBoxFiles.SelectedItem.ToString();

            // Wyciąganie czystej nazwy pliku
            string fileName = "";
            if (txt.StartsWith("Skopiowano: ")) fileName = txt.Substring("Skopiowano: ".Length);
            else if (txt.StartsWith("Pobrano: ")) fileName = txt.Substring("Pobrano: ".Length);
            else if (txt.StartsWith("Pobrano z tel: ")) fileName = txt.Substring("Pobrano z tel: ".Length);
            else fileName = txt;

            string path = Path.Combine(_targetPath, fileName);

            // Podgląd tylko dla obrazków
            if (File.Exists(path) && ImageExtensions.Contains(Path.GetExtension(path).ToUpperInvariant()))
            {
                try
                {
                    using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                        pictureBoxPreview.Image = Image.FromStream(fs);
                }
                catch { }
            }
        }

        private void SetUIState(bool enabled) { this.Enabled = enabled; }
        private void ResetUIState() { SetUIState(true); progressBarCopy.Visible = false; }
        private void ShowCriticalError(Exception ex) { MessageBox.Show($"Błąd: {ex.Message}"); }
    
        /// <summary>
        /// Włącza sprawdzanie pisowni po polsku dla wszystkich TextBoxów w formularzu
        /// </summary>
        private void EnableSpellCheckOnAllTextBoxes()
        {
            try
            {
                // Włącz sprawdzanie pisowni dla wszystkich kontrolek typu TextBox i RichTextBox
                foreach (Control control in GetAllControls(this))
                {
                    if (control is RichTextBox richTextBox)
                    {
                        richTextBox.EnableSpellCheck(true);
                    }
                    else if (control is TextBox textBox && !(textBox is SpellCheckTextBox))
                    {
                        // Dla zwykłych TextBoxów - bez podkreślania (bo nie obsługują kolorów)
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
}
}
