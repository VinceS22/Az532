using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace Az532
{
    class Program
    {
        
        static void Main(string[] args)
        {
            string connString = System.Configuration.ConfigurationManager.AppSettings.Get("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connString);
            WriteHeader("STORAGE ACCOUNT");
            GeneralStorageAccountSection(storageAccount, true);
            TableSection(storageAccount, true);
            BlobSection(storageAccount, true);
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }

        #region Storage Account Tables
        static void TableSection(CloudStorageAccount storageAccount, bool execute)
        {
            if(!execute)
            {
                return;
            }

            CloudTableClient cloudTableClient = storageAccount.CreateCloudTableClient();
            CloudTable cloudTable = cloudTableClient.GetTableReference("MyFirstTable");
            cloudTable.CreateIfNotExists();

            

            TableOperation retrieve = TableOperation.Retrieve<Car>("car","1");
            TableResult result = cloudTable.Execute(retrieve);

            if (result.Result == null)
            {
                Console.WriteLine("Car is not found");
            }
            else
            {
                var carObj = ((Car)result.Result);
                Console.WriteLine(string.Format("Car results: {0} {1}", carObj.Make, carObj.Model));

            }
            TableQuerySection(cloudTable);
        }

        static void ExecuteInserts(CloudTable cloudTable)
        {
            Car newCar = new Car(2, 2007, "Chevrolet", "Malibu", "Green");
            TableOperation insert = TableOperation.Insert(newCar);
            cloudTable.Execute(insert);
        }

        static void TableQuerySection(CloudTable table)
        {
            var tblFilter = TableQuery.GenerateFilterCondition("Make", QueryComparisons.Equal, "Chevrolet");
            TableQuery<Car> tableQuery = new TableQuery<Car>();

            IEnumerable<Car> tblQueryResults = table.ExecuteQuery(tableQuery);

            WriteHeader("TABLE QUERY");
            if (!tblQueryResults.Any())
            {
                Console.WriteLine("No Query Results Found");
            }

            foreach (var result in tblQueryResults)
            {
                Console.WriteLine(string.Format("Car results: {0} {1} {2} {3}", result.ID, result.Year, result.Make, result.Model));
            }
        }

        #endregion

        #region Storage Account Blobs
        static void BlobSection(CloudStorageAccount storageAccount, bool execute)
        {
            if (!execute)
            {
                return;
            }

            WriteHeader("BLOBS");
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("obj2");

            container.CreateIfNotExists();;
            //CopyBlob(container);
            UploadBlobSubdirectory(container);
        }
        static void UploadBlob(CloudBlobContainer container)
        {
            CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference("testFile");

            using (var fs = System.IO.File.OpenRead(@"C:\Users\Vince\Documents\trimps.txt"))
            {
                cloudBlockBlob.UploadFromStream(fs);
            }
        }

        static void CopyBlob(CloudBlobContainer container)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("testFile");
            var copyBlockBlob = container.GetBlockBlobReference("testFile-copy");
            copyBlockBlob.StartCopyAsync(new Uri(blockBlob.Uri.AbsoluteUri));
        }

        static void UploadBlobSubdirectory(CloudBlobContainer container)
        {
            var parentDirectory = container.GetDirectoryReference("parent");
            var childDirectory = parentDirectory.GetDirectoryReference("child1");
            // Seems it doesn't create this directory if I don't upload a file to it.
            var childDirectory2 = parentDirectory.GetDirectoryReference("child2");
            CloudBlockBlob blockBlob = childDirectory.GetBlockBlobReference("secondTestFile");
            using (var file = System.IO.File.OpenRead(@"C:\Users\Vince\Pictures\2017-10\IMG_1205.JPG"))
            {
                blockBlob.UploadFromStream(file);
            }

            Console.WriteLine("Upload complete to " + blockBlob.StorageUri.PrimaryUri);
        }

        #endregion

        #region General Storage Account Info

        static void GeneralStorageAccountSection(CloudStorageAccount storageAccount, bool execute)
        {
            if(!execute)
            {
                return;
            }

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("obj2");

            ListStorageAcctInfo(container);
            AddMetadata(container);
            ListMetadata(container);

        }

        static void ListStorageAcctInfo(CloudBlobContainer container)
        {
            WriteHeader("STORAGE ACCOUNT INFO");
            container.FetchAttributes();
            Console.WriteLine("Container name: " + container.StorageUri);
            Console.WriteLine("Last Modified: " + container.Properties.LastModified);

        }

        static void AddMetadata(CloudBlobContainer container)
        {
            container.Metadata.Clear();
            container.Metadata.Add("Author", "Vince");
            container.Metadata["relevantApp"] = "Trimps";
            container.SetMetadata();
        }

        static void ListMetadata(CloudBlobContainer container)
        {
            WriteHeader("METADATA");
            foreach (var metadata in container.Metadata)
            {
                Console.WriteLine(string.Format("{0}: {1}", metadata.Key, metadata.Value));
            }
        }

        #endregion

        #region Helpers

        static void WriteHeader(string heading)
        {
            Console.WriteLine(string.Format("-------{0}-------",heading));
        }
        #endregion

    }
}
