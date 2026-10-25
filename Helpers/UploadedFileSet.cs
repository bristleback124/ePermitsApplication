using ePermitsApp.DTOs;
using Microsoft.AspNetCore.Http;

namespace ePermitsApp.Helpers
{
    /// Files uploaded in chunks (via /api/uploads/permit-files) for one create/update request, grouped by document
    /// slot ("appinfo.reqdocproofownership"). Services take each slot's files where they save
    /// that slot, so required-file checks see them.
    public class UploadedFileSet
    {
        public static UploadedFileSet Empty => new(new Dictionary<string, List<FileMetadataDto>>());

        private readonly Dictionary<string, List<FileMetadataDto>> _bySlot;

        public UploadedFileSet(Dictionary<string, List<FileMetadataDto>> bySlot)
        {
            _bySlot = bySlot;
            Paths = bySlot.Values.SelectMany(files => files).Select(file => file.Path).Distinct().ToList();
        }

        /// Every blob path in the request, to tag as attached once the application is saved.
        public IReadOnlyList<string> Paths { get; }

        public List<FileMetadataDto> Take(string slot)
        {
            var key = slot.ToLowerInvariant();
            if (!_bySlot.TryGetValue(key, out var files))
            {
                return new List<FileMetadataDto>();
            }

            _bySlot.Remove(key);
            return files;
        }

        /// Throws if any slot wasn't taken, so a file is never silently dropped.
        public void EnsureAllTaken()
        {
            if (_bySlot.Count > 0)
            {
                throw new InvalidOperationException($"Uploaded files could not be attached to: {string.Join(", ", _bySlot.Keys)}.");
            }
        }

        public static string SlotName(string prefix, string propertyName) => $"{prefix}.{propertyName}".ToLowerInvariant();

        /// The document slots of a create-DTO section: its file properties, as "prefix.property".
        public static IEnumerable<string> SlotsOf(Type sectionDtoType, string prefix) =>
            sectionDtoType.GetProperties()
                .Where(property => property.PropertyType == typeof(IFormFileCollection) || property.PropertyType == typeof(IFormFile))
                .Select(property => SlotName(prefix, property.Name));
    }
}
