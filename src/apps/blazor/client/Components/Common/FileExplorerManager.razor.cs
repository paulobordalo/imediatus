using imediatus.Blazor.Infrastructure.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
using Syncfusion.Blazor.FileManager;
using Syncfusion.Blazor.Popups;
using System.Collections.ObjectModel;
using imediatus.Shared.Authorization;
using Nextended.Core.Extensions;
using imediatus.Shared.FileExplorer;

namespace imediatus.Blazor.Client.Components.Common;

public partial class FileExplorerManager
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthenticationService AuthService { get; set; } = default!;

    //[Inject]
    //protected IFileTypesClient FileTypesClient { get; set; } = default!;

    [Parameter]
    public Guid? ContainerId { get; set; }

    [Parameter]
    public bool IsNewFolder { get; set; }

    [Parameter]
    public bool IsUpload { get; set; }

    [Parameter]
    public bool IsDelete { get; set; }

    [Parameter]
    public bool IsDownload { get; set; }

    [Parameter]
    public bool IsRename { get; set; }

    #region Variables
    private SfFileManager? FileManager { get; set; } = default!;

    private string? _tenantKey { get; set; } = string.Empty;

    private bool IsPreview { get; set; }

    private string Title { get; set; } = "This type of file cannot be previewed.";
    private string FullFolderPath { get; set; } = string.Empty;

    protected ReadOnlyCollection<string>? ToolItems { get; set; }
    protected ReadOnlyCollection<string>? Files { get; set; }
    protected ReadOnlyCollection<string>? Folders { get; set; }
    protected ReadOnlyCollection<string>? Layouts { get; set; }
    #endregion

    protected override async Task OnInitializedAsync()
    {
        var user = (await AuthState).User;
        if (user.Identity?.IsAuthenticated == true)
        {
            _tenantKey = user.GetTenant();
        }

        #region ToolItems
        List<string> toolItems = new List<string>();
        if (IsNewFolder)
            toolItems.Add(FileExplorerConstants.NewFolder);
        if (IsUpload)
            toolItems.Add(FileExplorerConstants.Upload);
        if (IsDelete)
            toolItems.Add(FileExplorerConstants.Delete);
        if (IsDownload)
            toolItems.Add(FileExplorerConstants.Download);
        if (IsRename)
            toolItems.Add(FileExplorerConstants.Rename);
        if (IsNewFolder || IsDelete || IsDownload || IsRename)
            toolItems.Add(FileExplorerConstants.Separate);

        toolItems.AddRange([FileExplorerConstants.SortBy, FileExplorerConstants.View, FileExplorerConstants.Refresh, FileExplorerConstants.Selection, FileExplorerConstants.Details]);

        ToolItems = new ReadOnlyCollection<string>(toolItems);
        #endregion ToolItems

        #region FileMenu
        List<string> files = [FileExplorerConstants.Open, FileExplorerConstants.Separate];

        if (IsDelete)
            files.Add(FileExplorerConstants.Delete);
        if (IsDownload)
            files.Add(FileExplorerConstants.Download);
        if (IsRename)
            files.Add(FileExplorerConstants.Rename);
        if (IsDelete || IsDownload || IsRename)
            files.Add(FileExplorerConstants.Separate);
        
        files.Add(FileExplorerConstants.Details);

        Files = new ReadOnlyCollection<string>(files);
        #endregion FileMenu

        #region FolderMenu
        List<string> folders = [FileExplorerConstants.Open, FileExplorerConstants.Separate];

        if (IsDelete)
            folders.Add(FileExplorerConstants.Delete);
        if (IsDownload)
            folders.Add(FileExplorerConstants.Download);
        if (IsRename)
            folders.Add(FileExplorerConstants.Rename);
        if (IsDelete || IsDownload || IsRename)
            folders.Add(FileExplorerConstants.Separate);

        folders.Add(FileExplorerConstants.Details);

        Folders = new ReadOnlyCollection<string>(folders);
        #endregion FolderMenu

        #region LayoutMenu
        List<string> layouts = [FileExplorerConstants.SortBy, FileExplorerConstants.View, FileExplorerConstants.Refresh, FileExplorerConstants.Separate];
        
        if (IsNewFolder)
            layouts.Add(FileExplorerConstants.NewFolder);
        if (IsUpload)
            layouts.Add(FileExplorerConstants.Upload);
        if (IsNewFolder || IsUpload)
            layouts.Add(FileExplorerConstants.Separate);
        
        layouts.AddRange([ FileExplorerConstants.Details, FileExplorerConstants.SelectAll ]);

        Layouts = new ReadOnlyCollection<string>(layouts);
        #endregion LayoutMenu
    }

    public void BeforeSend(BeforeSendEventArgs args)
    {
        if (args.Action != "Upload")
        {
            string ajaxSettingsString = JsonConvert.SerializeObject(args.AjaxSettings);
            Dictionary<string, dynamic>? ajaxSettings = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(ajaxSettingsString);
            string dataString = ajaxSettings != null ? ajaxSettings["data"] : string.Empty;
            Dictionary<string, dynamic>? data = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(dataString);
            if (data is not null)
            {
                data.Add("LicenseKey", value: _tenantKey ?? string.Empty);
                data.Add("ContainerId", ContainerId ?? Guid.Empty);
            }

            if (ajaxSettings is not null)
            {
                ajaxSettings["data"] = JsonConvert.SerializeObject(data);
                string returnString = JsonConvert.SerializeObject(ajaxSettings);
                args.AjaxSettings = JsonConvert.DeserializeObject<object>(returnString);
            }
        }
    }

    private async Task FileOpen(FileOpenEventArgs args)
    {
        string dataString = JsonConvert.SerializeObject(args.FileDetails);
        Dictionary<string, dynamic>? fileDetails = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(dataString);
        if (fileDetails?["isFile"])
        {
            string filePath = fileDetails.ContainsKey("filterPath") && !string.IsNullOrEmpty(fileDetails["filterPath"]) ? fileDetails["filterPath"] : "/";
            Title = fileDetails.TryGetValue("name", out var value) ? string.Concat(filePath, value) : "This type of file cannot be previewed.";
            FullFolderPath = string.Concat("/", ContainerId.HasValue ? ContainerId.ToString() : string.Empty, Title);
            string fileExt = Path.GetExtension(Title.ToUpperInvariant()).ToUpperInvariant() ?? string.Empty;
            IsPreview = false;
            //if (!fileExt.CompareMultiple(".JPG", ".JPEG", ".GIF", ".PNG"))
            //{
            //    var filter = new SearchFileTypesRequest
            //    {
            //        PageSize = 10,
            //        Extension = fileExt
            //    };
            //    if (await ApiHelper.ExecuteCallGuardedAsync(() => FileTypesClient.SearchAsync(filter), Snackbar) is PaginationResponseOfFileTypeDto response)
            //    {
            //        var fileType = response.Data.FirstOrDefault();
            //        if (fileType != null)
            //        {
            //            if (fileType.Preview == PreviewEnum.Office.Name)
            //                IsPreview = true;

            //            if (fileType.Preview == PreviewEnum.Google.Name)
            //                IsPreview = true;

            //            if (fileType.Preview == PreviewEnum.Audio.Name)
            //                IsPreview = true;

            //            if (fileType.Preview == PreviewEnum.Image.Name)
            //                IsPreview = true;

            //            if (fileType.Preview == PreviewEnum.Video.Name)
            //                IsPreview = true;

            //            if (fileType.Preview == PreviewEnum.NoPreview.Name)
            //                IsPreview = false;
            //        }
            //    }
            //}
        }
    }
}
