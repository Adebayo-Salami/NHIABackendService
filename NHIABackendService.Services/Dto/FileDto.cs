using System;
using System.Text.Json.Serialization;

#nullable disable

namespace NHIABackendService.Services.Dto
{
    public class FileDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public string Path { get; set; }

        public string ContentType { get; set; }
    }
}
