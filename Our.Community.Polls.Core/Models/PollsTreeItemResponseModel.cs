using Umbraco.Cms.Api.Management.ViewModels;

namespace Our.Community.Polls.Core.Models
{
    public class PollsTreeItemResponseModel
    {
        public Guid Id { get; set; }

        public ReferenceByIdModel? Parent { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;


        public bool HasChildren { get; set; }
    }

}
