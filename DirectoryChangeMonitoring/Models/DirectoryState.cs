using System.IO;

namespace DirectoryChangeMonitoring.Models
{
    public class DirectoryState
    {
        public List<FileDetail> Files { get; set; } = new List<FileDetail>();

        public static DirectoryState Load(string path)
        {
            var state = new DirectoryState();
            LoadFilesRecursively(state, path);
            return state;
        }
        private static void LoadFilesRecursively(DirectoryState state, string directoryPath)
        {
            //Loads all files under selected directory
            foreach (var file in Directory.GetFiles(directoryPath))
            {
                state.Files.Add(new FileDetail
                {
                    FileName = Path.GetFileName(file),
                    FilePath = file,
                    LastModified = File.GetLastWriteTime(file),
                    Version = 1
                });
            }
            //Call another loading for each subdirectory
            foreach (var directory in Directory.GetDirectories(directoryPath))
            {
                LoadFilesRecursively(state, directory);
            }
        }

        public (List<FileDetail> NewFiles, List<FileDetail> ChangedFiles, List<FileDetail> DeletedFiles) Compare(DirectoryState previousState)
        {
            var newFiles = new List<FileDetail>();
            var changedFiles = new List<FileDetail>();
            var deletedFiles = new List<FileDetail>();

            var previousFilesDict = previousState.Files.ToDictionary(f => f.FilePath);

            foreach (var file in Files)
            {
                if (previousFilesDict.TryGetValue(file.FilePath, out var prevFile))
                {
                    if (file.LastModified > prevFile.LastModified)//Checks if the File was modified
                    {
                        file.Version = prevFile.Version + 1;
                        changedFiles.Add(file); //Modified File
                    }
                }
                else 
                {
                    newFiles.Add(file); //New File
                }
            }

            var currentFilesDict = Files.ToDictionary(f => f.FilePath);
            foreach (var prevFile in previousState.Files)
            {
                if (!currentFilesDict.ContainsKey(prevFile.FilePath))
                {
                    deletedFiles.Add(prevFile); //Deleted File
                }
            }

            return (newFiles, changedFiles, deletedFiles);
        }
    }
}
