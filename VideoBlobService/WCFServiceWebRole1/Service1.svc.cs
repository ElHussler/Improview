using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
// Azure Blobs
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
// Add Memory Stream from which we can upload the file
using System.IO;
// Allow asynchronous calls to upload from Improview
using System.Threading.Tasks;

namespace WCFServiceWebRole1
{
    public class Service1 : IService1
    {
        public String SaveFile(string guid, string filename, byte[] sentFile)
        {
            // Use live Azure storage account set up as a connectionString named 'Azure'
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("azure"));
            
            // Create a new instance of the blob storage client
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to user's container, based on their Improview 'userID'
            CloudBlobContainer container = blobClient.GetContainerReference(guid);     
       
            // Create the container if one doesnt already exist with the guid
            container.CreateIfNotExists();

            // Set access permission of container to 'public'
            BlobContainerPermissions containerPermissions = new BlobContainerPermissions();
            containerPermissions.PublicAccess = BlobContainerPublicAccessType.Container;
            container.SetPermissions(containerPermissions);

            // Retrieve a blobname reference for new video, creating or using 'answervideos' directory in container
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(string.Format("{0}/{1}", "answervideos", filename));

            // Create or overwrite the blob with file data using stream constructed with posted byte array
            using (Stream fileStream = new MemoryStream(sentFile))
            {
                blockBlob.UploadFromStream(fileStream);
            }

            // Get uri of file stored as blob
            String blobUrl = blockBlob.Uri.ToString();

            return blobUrl;
        }
    }
}