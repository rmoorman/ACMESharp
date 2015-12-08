﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACMESharp.Providers.AWS
{
    public class Route53Helper
    {
        public string HostedZoneId
        { get; set; }

        public int ResourceRecordTtl
        { get; set; } = 300;

        public AwsCommonParams CommonParams
        { get; set; } = new AwsCommonParams();

        public void EditTxtRecord(string dnsName, IEnumerable<string> dnsValues, bool delete = false)
        {
            var dnsValuesJoined = string.Join("\" \"", dnsValues);
            var rrset = new Amazon.Route53.Model.ResourceRecordSet
            {
                TTL = ResourceRecordTtl,
                Name = dnsName,
                Type = Amazon.Route53.RRType.TXT,
                ResourceRecords = new List<Amazon.Route53.Model.ResourceRecord>
                {
                    new Amazon.Route53.Model.ResourceRecord(
                            $"\"{dnsValuesJoined}\"")
                }
            };

            EditR53Record(rrset);
        }

        public void EditARecord(string dnsName, string dnsValue, bool delete = false)
        {
            var rrset = new Amazon.Route53.Model.ResourceRecordSet
            {
                TTL = ResourceRecordTtl,
                Name = dnsName,
                Type = Amazon.Route53.RRType.A,
                ResourceRecords = new List<Amazon.Route53.Model.ResourceRecord>
                {
                    new Amazon.Route53.Model.ResourceRecord(dnsValue)
                }
            };

            EditR53Record(rrset);
        }

        public void EditCnameRecord(string dnsName, string dnsValue, bool delete = false)
        {
            var rrset = new Amazon.Route53.Model.ResourceRecordSet
            {
                TTL = ResourceRecordTtl,
                Name = dnsName,
                Type = Amazon.Route53.RRType.CNAME,
                ResourceRecords = new List<Amazon.Route53.Model.ResourceRecord>
                {
                    new Amazon.Route53.Model.ResourceRecord(dnsValue)
                }
            };

            EditR53Record(rrset);
        }

        public void EditR53Record(Amazon.Route53.Model.ResourceRecordSet rrset, bool delete = false)
        {
            using (var r53 = new Amazon.Route53.AmazonRoute53Client(
                    CommonParams.AccessKeyId, CommonParams.SecretAccessKey,
                    CommonParams.RegionEndpoint))
            {
                var rrRequ = new Amazon.Route53.Model.ChangeResourceRecordSetsRequest
                {
                    HostedZoneId = HostedZoneId,
                    ChangeBatch = new Amazon.Route53.Model.ChangeBatch
                    {
                        Changes = new List<Amazon.Route53.Model.Change>
                        {
                            new Amazon.Route53.Model.Change
                            {
                                Action = delete
                                    ? Amazon.Route53.ChangeAction.DELETE
                                    : Amazon.Route53.ChangeAction.UPSERT,
                                ResourceRecordSet = rrset
                            }
                        }
                    }
                };
                var rrResp = r53.ChangeResourceRecordSets(rrRequ);
            }
        }
    }
}