﻿using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;

namespace Pluralsight.AWS.IAM.NetSDK.WinUI
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Amazon;
    using Amazon.EC2;
    using Amazon.EC2.Model;
    using Amazon.SimpleDB;
    using Amazon.SimpleDB.Model;
    using Amazon.S3;
    using Amazon.S3.Model;

    public sealed class Program
    {
        public static void Main(string[] args)
        {
            //Console.Write(GetServiceOutput());
            AddPolicy();
            Console.Read();
        }

        private static void AddPolicy()
        {
            var client = new AmazonIdentityManagementServiceClient();
            var policyRequest = new PutUserPolicyRequest
            {
                UserName = "mundySDK",
                PolicyName = "EC2Policy",
                PolicyDocument =
                    "{\"Version\": \"2012-10-17\",\"Statement\": [ {\"Sid\": \"Stmt1453760589829\",\"Action\": [\"ec2:DescribeInstances\"],\"Effect\": \"Allow\",\"Resource\":\"*\"}]}"
            };

            var policyResponse = client.PutUserPolicy(policyRequest);
            Console.WriteLine("Policy Added");
        }

        private static void CreateUser()
        {
            Console.WriteLine("** Create User **");
            var iamClient = new AmazonIdentityManagementServiceClient();
            var request = new CreateUserRequest
            {
                UserName = "mundySDK",
                Path = @"/IT/architecture/"
            };

            var response = iamClient.CreateUser(request);

            Console.WriteLine("User Created");

        }
         
        public static string GetServiceOutput()
        {
            var sb = new StringBuilder(1024);
            using (var sr = new StringWriter(sb))
            {
                sr.WriteLine("===========================================");
                sr.WriteLine("Welcome to the AWS .NET SDK!");
                sr.WriteLine("===========================================");

                // Print the number of Amazon EC2 instances.
                IAmazonEC2 ec2 = new AmazonEC2Client();
                var ec2Request = new DescribeInstancesRequest();

                try
                {
                    var ec2Response = ec2.DescribeInstances(ec2Request);
                    var numInstances = 0;
                    numInstances = ec2Response.Reservations.Count;
                    sr.WriteLine(string.Format("You have {0} Amazon EC2 instance(s) running in the {1} region.",
                                               numInstances, ConfigurationManager.AppSettings["AWSRegion"]));
                }
                catch (AmazonEC2Exception ex)
                {
                    if (ex.ErrorCode != null && ex.ErrorCode.Equals("AuthFailure"))
                    {
                        sr.WriteLine("The account you are using is not signed up for Amazon EC2.");
                        sr.WriteLine("You can sign up for Amazon EC2 at http://aws.amazon.com/ec2");
                    }
                    else
                    {
                        sr.WriteLine("Caught Exception: " + ex.Message);
                        sr.WriteLine("Response Status Code: " + ex.StatusCode);
                        sr.WriteLine("Error Code: " + ex.ErrorCode);
                        sr.WriteLine("Error Type: " + ex.ErrorType);
                        sr.WriteLine("Request ID: " + ex.RequestId);
                    }
                }
                sr.WriteLine();

                // Print the number of Amazon SimpleDB domains.
                IAmazonSimpleDB sdb = new AmazonSimpleDBClient();
                var sdbRequest = new ListDomainsRequest();

                try
                {
                    var sdbResponse = sdb.ListDomains(sdbRequest);

                    var numDomains = 0;
                    numDomains = sdbResponse.DomainNames.Count;
                    sr.WriteLine(string.Format("You have {0} Amazon SimpleDB domain(s) in the {1} region.",
                                               numDomains, ConfigurationManager.AppSettings["AWSRegion"]));
                }
                catch (AmazonSimpleDBException ex)
                {
                    if (ex.ErrorCode != null && ex.ErrorCode.Equals("AuthFailure"))
                    {
                        sr.WriteLine("The account you are using is not signed up for Amazon SimpleDB.");
                        sr.WriteLine("You can sign up for Amazon SimpleDB at http://aws.amazon.com/simpledb");
                    }
                    else
                    {
                        sr.WriteLine("Caught Exception: " + ex.Message);
                        sr.WriteLine("Response Status Code: " + ex.StatusCode);
                        sr.WriteLine("Error Code: " + ex.ErrorCode);
                        sr.WriteLine("Error Type: " + ex.ErrorType);
                        sr.WriteLine("Request ID: " + ex.RequestId);
                    }
                }
                sr.WriteLine();

                // Print the number of Amazon S3 Buckets.
                IAmazonS3 s3Client = new AmazonS3Client();

                try
                {
                    var response = s3Client.ListBuckets();
                    var numBuckets = 0;
                    if (response.Buckets != null &&
                        response.Buckets.Count > 0)
                    {
                        numBuckets = response.Buckets.Count;
                    }
                    sr.WriteLine("You have " + numBuckets + " Amazon S3 bucket(s).");
                }
                catch (AmazonS3Exception ex)
                {
                    if (ex.ErrorCode != null && (ex.ErrorCode.Equals("InvalidAccessKeyId") ||
                        ex.ErrorCode.Equals("InvalidSecurity")))
                    {
                        sr.WriteLine("Please check the provided AWS Credentials.");
                        sr.WriteLine("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                    }
                    else
                    {
                        sr.WriteLine("Caught Exception: " + ex.Message);
                        sr.WriteLine("Response Status Code: " + ex.StatusCode);
                        sr.WriteLine("Error Code: " + ex.ErrorCode);
                        sr.WriteLine("Request ID: " + ex.RequestId);
                    }
                }
                sr.WriteLine("Press any key to continue...");
            }
            return sb.ToString();
        }
    }
}