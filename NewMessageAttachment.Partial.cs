// NewMessageAttachment.Partial.cs
using Reklamacje_Dane.Allegro.Issues;

namespace Reklamacje_Dane.Allegro.Issues
{
    // Rozszerzamy istniejący model o pomocniczą właściwość na potrzeby UI.
    // API jej nie wymaga – to wygoda do wklejenia linku w wiadomości.
    public partial class NewMessageAttachment
    {
        public string Url { get; set; }
    }
}
