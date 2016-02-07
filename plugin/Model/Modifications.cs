namespace MusicBeePlugin.Model
{
    public class Modifications
    {
        public Modifications(string[] deletedFiles, string[] newFiles, string[] updatedFiles)
        {
            this.DeletedFiles = deletedFiles;
            this.NewFiles = newFiles;
            this.UpdatedFiles = updatedFiles;
        }

        public string[] DeletedFiles { get; }

        public string[] NewFiles { get; }

        public string[] UpdatedFiles { get; }
    }
}