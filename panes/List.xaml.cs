using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace panes
{
    public partial class Page2 : PhoneApplicationPage
    {

        private static void List(AmazonS3Client client)
        {
                        // Issue call
                ListBucketsResponse listResponse = client.ListBuckets();
            // View response data
            Console.WriteLine("Buckets owner - {0}", listResponse.Owner.DisplayName);
            foreach (S3Bucket bucket in listResponse.Buckets)
            {
                Console.WriteLine("Bucket {0}, Created on {1}", bucket.BucketName, bucket.CreationDate);
            }
        }
        private void PutBuck(AmazonS3Client client)
        {
            // Construct request
            PutBucketRequest request = new PutBucketRequest
            {
                BucketName = "com.loofah.photos",
                BucketRegion = S3Region.US,         // set region to US
                CannedACL = S3CannedACL.PublicRead  // make bucket publicly readable
            };
            // Issue call
            PutBucketResponse putBucketResponse = client.PutBucket(request);
        }
        private static string PutObj(AmazonS3Client client)
        {
            string time = DateTime.Now.ToString("hhmmsstt");
            // Create a PutObject request
            PutObjectRequest putObjRequest = new PutObjectRequest
            {
                BucketName = "com.loofah.photos",
                Key = time,
                FilePath = "C:\\Users\\Ryan\\Pictures\\cz1jb.jpg"
            };
            // Put object
            PutObjectResponse putObjResponse = client.PutObject(putObjRequest);
            return time;
        }
        public static void Main(string[] args)
        {
            // Create a client
            AmazonS3Client client = new AmazonS3Client();
            string time = PutObj(client);
            // Create a GetObject request
            GetObjectRequest getObjRequest = new GetObjectRequest
            {
                BucketName = "com.loofah.photos",
                Key = time
            };
            System.Console.WriteLine(time);
            // Issue request and remember to dispose of the response
            using (GetObjectResponse getObjResponse = client.GetObject(getObjRequest))
            {
                getObjResponse.WriteResponseStreamToFile("C:\\Users\\Ryan\\Pictures\\" + time + ".jpg", false);
            }
            System.Console.Read();
        }

        public Page2()
        {
            InitializeComponent();


        }
    }
}