using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using System.Collections.Generic;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Linq;
using System.Security.Policy;

public class ProvisionDisplayTemplate
{
    public ProvisionDisplayTemplate()
	{
	}
    public string[] folderUrls = { "_catalogs/masterpage/Display Templates/Search" };

    public void DisplayTemplate()
    {
    SPSite site = properties.Feature.Parent as SPSite; 
            if (site != null) 
            { 
                SPWeb rootWeb = site.RootWeb;
                SPList gallery = site.GetCatalog(SPListTemplateType.MasterPageCatalog);
                if (gallery != null) 
                { 
                    SPListItemCollection folders = gallery.Folders; 
                    string featureId = properties.Feature.Definition.Id.ToString();
                    foreach (string folderUrl in folderUrls) 
                    { 
                        SPFolder folder = GetFolderByUrl(folders, folderUrl); 
                        if (folder != null) 
                        { 
                            PublishFiles(folder, featureId); 
                        } 
                    } 
                } 
            }
      }
    private static SPFolder GetFolderByUrl(SPListItemCollection folders, string folderUrl)
    {
        if (folders == null)
        {
            throw new ArgumentNullException("folders");
        }
        if (String.IsNullOrEmpty(folderUrl))
        {
            throw new ArgumentNullException("folderUrl");
        }
        SPFolder folder = null;

        SPListItem item = (from SPListItem i
                           in folders
                           where i.Url.Equals(folderUrl, StringComparison.InvariantCultureIgnoreCase)
                           select i).FirstOrDefault();
        if (item != null)
        {
            folder = item.Folder;
        }
        return folder;
    }
    private static void PublishFiles(SPFolder folder, string featureId)
    {
        if (folder == null)
        {
            throw new ArgumentNullException("folder");
        }
        if (String.IsNullOrEmpty(featureId))
        {
            throw new ArgumentNullException("featureId");
        }
        SPFileCollection files = folder.Files;
        var drafts = from SPFile f
                              in files
                     where String.Equals(f.Properties["FeatureId"] as string, featureId, StringComparison.InvariantCultureIgnoreCase) &&
                     f.Level == SPFileLevel.Draft
                     select f;
        foreach (SPFile f in drafts)
        {
            f.Publish("");
            f.Update();
        }

    }

}
