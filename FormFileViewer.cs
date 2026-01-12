using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Web.WebView2.Core; // <— ważne!

// Użycie aliasów, aby uniknąć konfliktów nazw między WPF a WinForms
using WpfControls = System.Windows.Controls;
using WpfMedia = System.Windows.Media;

namespace Reklamacje_Dane
{
    public partial class FormFileViewer : Form
    {
        private readonly string _complaintFolderPath;

        private static readonly string[] ImageExtensions = { ".JPG", ".JPEG", ".PNG", ".BMP", ".GIF", ".TIF", ".TIFF" };
        private static readonly string[] VideoExtensions = { ".MP4", ".AVI", ".WMV", ".MOV", ".MKV" };
        private static readonly string[] WebViewCompatibleExtensions = { ".PDF", ".TXT", ".HTML", ".HTM", ".XML" };

        private WpfControls.MediaElement _mediaElement;

        // Pan/zoom dla obrazu
        private bool _isDragging = false;
        private Point _dragStartPoint;

        // Trzymamy żywy bufor obrazu, by nie blokować pliku na dysku
        private MemoryStream _currentImageStream;

        public FormFileViewer(string nrZgloszenia)
        {
            InitializeComponent();
            Text = $"Przeglądarka plików: {nrZgloszenia}";
            _complaintFolderPath = Path.Combine(Application.StartupPath, "Dane", nrZgloszenia.Replace('/', '.'));

            InitializeWpfControls();
            InitializeWebView2Async();
            InitializeImageControls();
            InitializeListViewUi();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void InitializeWpfControls()
        {
            _mediaElement = new WpfControls.MediaElement
            {
                LoadedBehavior = WpfControls.MediaState.Manual,
                UnloadedBehavior = WpfControls.MediaState.Close,
                Stretch = WpfMedia.Stretch.Uniform
            };
            elementHostVideo.Child = _mediaElement;
        }

        private void InitializeListViewUi()
        {
            listViewFiles.View = View.LargeIcon;
            listViewFiles.Sorting = SortOrder.Ascending;
            listViewFiles.MultiSelect = false;
            listViewFiles.ItemActivate += (s, e) => btnOpen.PerformClick(); // dwuklik = otwórz
        }

        private void InitializeImageControls()
        {
            pictureBoxPreview.MouseWheel += PictureBoxPreview_MouseWheel;
            pictureBoxPreview.MouseDown += PictureBoxPreview_MouseDown;
            pictureBoxPreview.MouseMove += PictureBoxPreview_MouseMove;
            pictureBoxPreview.MouseUp += PictureBoxPreview_MouseUp;
        }

        // ★ KLUCZOWE: lokalny UserDataFolder, żeby nie kolidować na dysku sieciowym
        private async void InitializeWebView2Async()
        {
            try
            {
                // %LOCALAPPDATA%\ReklamacjeDane\WebView2\<user>@<pc>\proc_<pid>
                string baseLocal = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "ReklamacjeDane", "WebView2");

                string instanceDir = Path.Combine(
                    baseLocal,
                    $"{Environment.UserName}@{Environment.MachineName}",
                    $"proc_{Process.GetCurrentProcess().Id}");

                Directory.CreateDirectory(instanceDir);

                // Zamiast CreationProperties: tworzymy środowisko z lokalnym UserDataFolder
                var env = await CoreWebView2Environment.CreateAsync(
                    browserExecutableFolder: null,          // użyje zainstalowanego WebView2 Runtime
                    userDataFolder: instanceDir,            // LOKALNY katalog per user/PC/proces
                    options: new CoreWebView2EnvironmentOptions()
                );

                await webView2Preview.EnsureCoreWebView2Async(env);

                if (webView2Preview.CoreWebView2 != null)
                {
                    webView2Preview.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
                    webView2Preview.CoreWebView2.Settings.AreDevToolsEnabled = false;
                    webView2Preview.CoreWebView2.Settings.IsZoomControlEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Nie można zainicjować komponentu WebView2. Podgląd PDF/HTML/TXT może nie działać.\n" +
                    $"Błąd: {ex.Message}",
                    "Błąd krytyczny",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                webView2Preview.Visible = false;
                lblPreviewNotAvailable.Visible = true;
                lblPreviewNotAvailable.Text = "Podgląd niedostępny.\r\nUżyj przycisku 'Otwórz'.";
            }
        
        }

        private void FormFileViewer_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(_complaintFolderPath))
            {
                MessageBox.Show("Folder z załącznikami dla tego zgłoszenia nie istnieje.", "Brak folderu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }
            LoadFiles();
        }

        private void LoadFiles()
        {
            listViewFiles.BeginUpdate();
            try
            {
                listViewFiles.Items.Clear();
                imageListLarge.Images.Clear();

                if (!Directory.Exists(_complaintFolderPath)) return;

                var files = Directory.GetFiles(_complaintFolderPath)
                                     .OrderBy(Path.GetFileName)
                                     .ToArray();

                foreach (var file in files)
                {
                    try
                    {
                        var icon = IconTools.GetFileIcon(file, true);
                        var key = Path.GetFileName(file);
                        imageListLarge.Images.Add(key, icon);

                        var lvi = new ListViewItem
                        {
                            Text = key,
                            ImageKey = key,
                            Tag = file
                        };
                        listViewFiles.Items.Add(lvi);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Icon load error '{file}': {ex.Message}");
                    }
                }
            }
            finally
            {
                listViewFiles.EndUpdate();
            }
        }

        private void listViewFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            StopVideo();

            if (listViewFiles.SelectedItems.Count == 0)
            {
                ClearAllPreviews();
                btnOpen.Enabled = false;
                btnDelete.Enabled = false;
                lblFileName.Text = "Wybierz plik z listy...";
                return;
            }

            btnOpen.Enabled = true;
            btnDelete.Enabled = true;

            var sel = listViewFiles.SelectedItems[0];
            string filePath = sel.Tag.ToString();
            lblFileName.Text = Path.GetFileName(filePath);

            DisplayPreview(filePath);
        }

        private void DisplayPreview(string filePath)
        {
            ClearAllPreviews();
            string ext = Path.GetExtension(filePath).ToUpperInvariant();

            if (ImageExtensions.Contains(ext))
            {
                panelImageContainer.Visible = true;
                btnRotateLeft.Visible = true;
                btnRotateRight.Visible = true;

                try
                {
                    DisposeCurrentImageStream();
                    var bytes = File.ReadAllBytes(filePath);
                    _currentImageStream = new MemoryStream(bytes, writable: false);
                    var img = Image.FromStream(_currentImageStream, useEmbeddedColorManagement: true, validateImageData: true);

                    pictureBoxPreview.Image = img;
                    pictureBoxPreview.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBoxPreview.Size = panelImageContainer.ClientSize;
                    pictureBoxPreview.Location = new Point(0, 0);

                    lblPreviewNotAvailable.Visible = false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    lblPreviewNotAvailable.Text = "Podgląd obrazu niedostępny.\r\nUżyj przycisku 'Otwórz'.";
                    lblPreviewNotAvailable.Visible = true;
                }
            }
            else if (VideoExtensions.Contains(ext))
            {
                elementHostVideo.Visible = true;
                try
                {
                    _mediaElement.Source = new Uri(filePath);
                    _mediaElement.Position = TimeSpan.Zero;
                    _mediaElement.Play();
                    lblPreviewNotAvailable.Visible = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Nie można odtworzyć pliku wideo.\n\nMożliwy brak kodeków w systemie.\n" +
                        $"Szczegóły: {ex.Message}",
                        "Błąd odtwarzania wideo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblPreviewNotAvailable.Visible = true;
                }
            }
            else if (WebViewCompatibleExtensions.Contains(ext))
            {
                webView2Preview.Visible = true;
                try
                {
                    if (webView2Preview.CoreWebView2 != null)
                    {
                        var uri = new Uri(filePath).AbsoluteUri; // file:///…
                        webView2Preview.CoreWebView2.Navigate(uri);
                        lblPreviewNotAvailable.Visible = false;
                    }
                    else
                    {
                        InitializeWebView2Async();
                        lblPreviewNotAvailable.Text = "Trwa inicjalizacja podglądu…";
                        lblPreviewNotAvailable.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    lblPreviewNotAvailable.Text = "Podgląd niedostępny.\r\nUżyj przycisku 'Otwórz'.";
                    lblPreviewNotAvailable.Visible = true;
                }
            }
            else
            {
                lblPreviewNotAvailable.Visible = true;
                lblPreviewNotAvailable.Text = "Podgląd niedostępny.\r\nUżyj przycisku 'Otwórz'.";
            }
        }

        private void ClearAllPreviews()
        {
            // Obraz
            panelImageContainer.Visible = false;
            if (pictureBoxPreview.Image != null)
            {
                pictureBoxPreview.Image.Dispose();
                pictureBoxPreview.Image = null;
            }
            DisposeCurrentImageStream();
            btnRotateLeft.Visible = false;
            btnRotateRight.Visible = false;

            // Wideo
            elementHostVideo.Visible = false;
            if (_mediaElement != null)
            {
                try { _mediaElement.Stop(); _mediaElement.Source = null; } catch { }
            }

            // WebView
            webView2Preview.Visible = false;
            try
            {
                if (webView2Preview.CoreWebView2 != null)
                    webView2Preview.CoreWebView2.Navigate("about:blank");
            }
            catch { }

            lblPreviewNotAvailable.Visible = true;
            lblPreviewNotAvailable.Text = "Wybierz plik z listy po lewej stronie.";
        }

        private void DisposeCurrentImageStream()
        {
            if (_currentImageStream != null)
            {
                try { _currentImageStream.Dispose(); } catch { }
                _currentImageStream = null;
            }
        }

        private void StopVideo()
        {
            if (_mediaElement != null)
            {
                try { _mediaElement.Stop(); _mediaElement.Source = null; } catch { }
            }
        }

        #region Image Pan, Zoom, Rotate

        private void btnRotateRight_Click(object sender, EventArgs e)
        {
            if (pictureBoxPreview.Image == null) return;
            pictureBoxPreview.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            pictureBoxPreview.Refresh();
            NormalizeImageSizeIfTooSmall();
        }

        private void btnRotateLeft_Click(object sender, EventArgs e)
        {
            if (pictureBoxPreview.Image == null) return;
            pictureBoxPreview.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
            pictureBoxPreview.Refresh();
            NormalizeImageSizeIfTooSmall();
        }

        private void PictureBoxPreview_MouseWheel(object sender, MouseEventArgs e)
        {
            if (pictureBoxPreview.Image == null) return;
            double zoom = e.Delta < 0 ? 1 / 1.25 : 1.25;

            int newW = (int)(pictureBoxPreview.Width * zoom);
            int newH = (int)(pictureBoxPreview.Height * zoom);

            if (newW < panelImageContainer.Width)
            {
                newW = panelImageContainer.Width;
                newH = (int)((double)pictureBoxPreview.Image.Height / pictureBoxPreview.Image.Width * newW);
            }

            pictureBoxPreview.Size = new Size(newW, newH);
            ClampImageInsideContainer();
        }

        private void PictureBoxPreview_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && pictureBoxPreview.Image != null)
            {
                _isDragging = true;
                _dragStartPoint = e.Location;
                pictureBoxPreview.Cursor = Cursors.Hand;
            }
        }

        private void PictureBoxPreview_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                int dx = e.Location.X - _dragStartPoint.X;
                int dy = e.Location.Y - _dragStartPoint.Y;
                pictureBoxPreview.Location = new Point(pictureBoxPreview.Left + dx, pictureBoxPreview.Top + dy);
                ClampImageInsideContainer();
            }
        }

        private void PictureBoxPreview_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = false;
                pictureBoxPreview.Cursor = Cursors.Default;
            }
        }

        private void ClampImageInsideContainer()
        {
            int minLeft = Math.Min(0, panelImageContainer.ClientSize.Width - pictureBoxPreview.Width);
            int minTop = Math.Min(0, panelImageContainer.ClientSize.Height - pictureBoxPreview.Height);

            int newLeft = Math.Max(minLeft, Math.Min(pictureBoxPreview.Left, 0));
            int newTop = Math.Max(minTop, Math.Min(pictureBoxPreview.Top, 0));
            pictureBoxPreview.Location = new Point(newLeft, newTop);
        }

        private void NormalizeImageSizeIfTooSmall()
        {
            if (pictureBoxPreview.Image == null) return;
            if (pictureBoxPreview.Width < panelImageContainer.Width)
            {
                pictureBoxPreview.Width = panelImageContainer.Width;
                pictureBoxPreview.Height = (int)((double)pictureBoxPreview.Image.Height / pictureBoxPreview.Image.Width * pictureBoxPreview.Width);
                ClampImageInsideContainer();
            }
        }

        #endregion

        #region Buttons

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (listViewFiles.SelectedItems.Count == 0) return;

            string filePath = listViewFiles.SelectedItems[0].Tag.ToString();
            try
            {
                var psi = new ProcessStartInfo { FileName = filePath, UseShellExecute = true };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie można otworzyć pliku.\nBłąd: {ex.Message}",
                    "Błąd otwierania", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listViewFiles.SelectedItems.Count == 0) return;

            string filePath = listViewFiles.SelectedItems[0].Tag.ToString();
            if (MessageBox.Show($"Czy na pewno chcesz usunąć plik '{Path.GetFileName(filePath)}'?",
                    "Potwierdzenie usunięcia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                StopVideo();
                if (pictureBoxPreview.Image != null)
                {
                    pictureBoxPreview.Image.Dispose();
                    pictureBoxPreview.Image = null;
                }
                DisposeCurrentImageStream();

                FileSystem.DeleteFile(filePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                LoadFiles();
                ClearAllPreviews();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie można usunąć pliku.\nBłąd: {ex.Message}",
                    "Błąd usuwania", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                var psi = new ProcessStartInfo { FileName = _complaintFolderPath, UseShellExecute = true };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie można otworzyć folderu.\nBłąd: {ex.Message}",
                    "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                if (webView2Preview != null)
                {
                    if (webView2Preview.CoreWebView2 != null)
                        webView2Preview.CoreWebView2.Navigate("about:blank");
                    webView2Preview.Dispose();
                }
            }
            catch { /* ignore */ }

            StopVideo();
            DisposeCurrentImageStream();
            base.OnFormClosing(e);
        }
    }

    public static class IconTools
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes,
            out SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        // NOWOŚĆ: Deklaracja funkcji do zwalniania zasobów ikony
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon; public int iIcon; public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)] public string szTypeName;
        }

        private const uint SHGFI_ICON = 0x100, SHGFI_LARGEICON = 0x0, SHGFI_USEFILEATTRIBUTES = 0x10;

        public static Icon GetFileIcon(string path, bool large)
        {
            Icon icon = null;
            IntPtr hIcon = IntPtr.Zero;
            try
            {
                uint flags = SHGFI_ICON | SHGFI_USEFILEATTRIBUTES;
                if (large)
                {
                    flags |= SHGFI_LARGEICON;
                }

                if (SHGetFileInfo(path, 0, out SHFILEINFO shfi, (uint)Marshal.SizeOf(typeof(SHFILEINFO)), flags) != IntPtr.Zero)
                {
                    if (shfi.hIcon != IntPtr.Zero)
                    {
                        hIcon = shfi.hIcon;
                        // Klonujemy ikonę, aby system mógł zwolnić oryginalną
                        icon = (Icon)Icon.FromHandle(hIcon).Clone();
                    }
                }
            }
            finally
            {
                // POPRAWKA: Zawsze niszczymy uchwyt do ikony otrzymany z systemu
                if (hIcon != IntPtr.Zero)
                {
                    DestroyIcon(hIcon);
                }
            }
            return icon;
        }
    
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
