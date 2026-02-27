using ePermitsApp.DTOs;

namespace ePermitsApp.Helpers
{
    public static class FilePathHelper
    {
        public static string Serialize(List<FileMetadataDto> files)
        {
            if (files == null || !files.Any()) return string.Empty;
            return string.Join("|", files.Select(f => $"{f.Name}::{f.Size}::{f.Path}"));
        }

        public static List<FileMetadataDto> Deserialize(string? data)
        {
            if (string.IsNullOrWhiteSpace(data)) return new List<FileMetadataDto>();

            var result = new List<FileMetadataDto>();
            var items = data.Split('|', StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in items)
            {
                var parts = item.Split("::");
                if (parts.Length == 3 && long.TryParse(parts[1], out long size))
                {
                    result.Add(new FileMetadataDto
                    {
                        Name = parts[0],
                        Size = size,
                        Path = parts[2]
                    });
                }
                else
                {
                    // Fallback for raw paths or incorrect format
                    result.Add(new FileMetadataDto
                    {
                        Name = Path.GetFileName(item),
                        Size = 0,
                        Path = item
                    });
                }
            }

            return result;
        }

        public static string SerializeSingle(FileMetadataDto? file)
        {
            if (file == null) return string.Empty;
            return Serialize(new List<FileMetadataDto> { file });
        }

        public static FileMetadataDto DeserializeSingle(string? data)
        {
            return Deserialize(data).FirstOrDefault() ?? new FileMetadataDto();
        }
    }
}
