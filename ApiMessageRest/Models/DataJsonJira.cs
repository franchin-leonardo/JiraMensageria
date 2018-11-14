using System;
using System.Collections.Generic;

namespace ApiMessageRest.Models
{
    public class DataJsonJira
    {
        public class AvatarUrls
        {
            public string _48x48 { get; set; }
            public string _24x24 { get; set; }
            public string _16x16 { get; set; }
            public string _32x32 { get; set; }
        }

        public class Author
        {
            public string self { get; set; }
            public string name { get; set; }
            public string key { get; set; }
            public string emailAddress { get; set; }
            public AvatarUrls avatarUrls { get; set; }
            public string displayName { get; set; }
            public bool active { get; set; }
            public string timeZone { get; set; }
            public string accountId { get; set; }
        }


        public class UpdateAuthor
        {
            public string self { get; set; }
            public string name { get; set; }
            public string key { get; set; }
            public string emailAddress { get; set; }
            public AvatarUrls avatarUrls { get; set; }
            public string displayName { get; set; }
            public bool active { get; set; }
            public string timeZone { get; set; }
            public string accountId { get; set; }
        }

        public class Worklog
        {
            public string self { get; set; }
            public Author author { get; set; }
            public UpdateAuthor updateAuthor { get; set; }
            public string comment { get; set; }
            public DateTime created { get; set; }
            public DateTime updated { get; set; }
            public DateTime started { get; set; }
            public string timeSpent { get; set; }
            public int timeSpentSeconds { get; set; }
            public string id { get; set; }
            public string issueId { get; set; }
        }

        public class FixVersion
        {
            public string self { get; set; }
            public string id { get; set; }
            public string description { get; set; }
            public string name { get; set; }
            public bool archived { get; set; }
            public bool released { get; set; }
            public string releaseDate { get; set; }
        }

        public class Priority
        {
            public string self { get; set; }
            public string iconUrl { get; set; }
            public string name { get; set; }
            public string id { get; set; }
        }

        public class Type
        {
            public string id { get; set; }
            public string name { get; set; }
            public string inward { get; set; }
            public string outward { get; set; }
            public string self { get; set; }
        }

        public class StatusCategory
        {
            public string self { get; set; }
            public int id { get; set; }
            public string key { get; set; }
            public string colorName { get; set; }
            public string name { get; set; }
        }

        public class Status
        {
            public string self { get; set; }
            public string description { get; set; }
            public string iconUrl { get; set; }
            public string name { get; set; }
            public string id { get; set; }
            public StatusCategory statusCategory { get; set; }
        }

        public class Priority2
        {
            public string self { get; set; }
            public string iconUrl { get; set; }
            public string name { get; set; }
            public string id { get; set; }
        }

        public class Issuetype
        {
            public string self { get; set; }
            public string id { get; set; }
            public string description { get; set; }
            public string iconUrl { get; set; }
            public string name { get; set; }
            public bool subtask { get; set; }
            public int avatarId { get; set; }
        }

        public class Fields2
        {
            public string summary { get; set; }
            public Status status { get; set; }
            public Priority2 priority { get; set; }
            public Issuetype issuetype { get; set; }
        }

        public class OutwardIssue
        {
            public string id { get; set; }
            public string key { get; set; }
            public string self { get; set; }
            public Fields2 fields { get; set; }
        }

        public class Issuelink
        {
            public string id { get; set; }
            public string self { get; set; }
            public Type type { get; set; }
            public OutwardIssue outwardIssue { get; set; }
        }

        public class Assignee
        {
            public string self { get; set; }
            public string name { get; set; }
            public string key { get; set; }
            public string emailAddress { get; set; }
            public AvatarUrls avatarUrls { get; set; }
            public string displayName { get; set; }
            public bool active { get; set; }
            public string timeZone { get; set; }
        }

        public class StatusCategory2
        {
            public string self { get; set; }
            public int id { get; set; }
            public string key { get; set; }
            public string colorName { get; set; }
            public string name { get; set; }
        }

        public class Status2
        {
            public string self { get; set; }
            public string description { get; set; }
            public string iconUrl { get; set; }
            public string name { get; set; }
            public string id { get; set; }
            public StatusCategory2 statusCategory { get; set; }
        }

        public class Creator
        {
            public string self { get; set; }
            public string name { get; set; }
            public string key { get; set; }
            public string emailAddress { get; set; }
            public AvatarUrls avatarUrls { get; set; }
            public string displayName { get; set; }
            public bool active { get; set; }
            public string timeZone { get; set; }
        }

        public class StatusCategory3
        {
            public string self { get; set; }
            public int id { get; set; }
            public string key { get; set; }
            public string colorName { get; set; }
            public string name { get; set; }
        }

        public class Status3
        {
            public string self { get; set; }
            public string description { get; set; }
            public string iconUrl { get; set; }
            public string name { get; set; }
            public string id { get; set; }
            public StatusCategory3 statusCategory { get; set; }
        }

        public class Priority3
        {
            public string self { get; set; }
            public string iconUrl { get; set; }
            public string name { get; set; }
            public string id { get; set; }
        }

        public class Issuetype2
        {
            public string self { get; set; }
            public string id { get; set; }
            public string description { get; set; }
            public string iconUrl { get; set; }
            public string name { get; set; }
            public bool subtask { get; set; }
            public int avatarId { get; set; }
        }

        public class Fields3
        {
            public string summary { get; set; }
            public Status3 status { get; set; }
            public Priority3 priority { get; set; }
            public Issuetype2 issuetype { get; set; }
        }

        public class Subtask
        {
            public string id { get; set; }
            public string key { get; set; }
            public string self { get; set; }
            public Fields3 fields { get; set; }
        }

        public class Reporter
        {
            public string self { get; set; }
            public string name { get; set; }
            public string key { get; set; }
            public string emailAddress { get; set; }
            public AvatarUrls avatarUrls { get; set; }
            public string displayName { get; set; }
            public bool active { get; set; }
            public string timeZone { get; set; }
        }

        public class Aggregateprogress
        {
            public int progress { get; set; }
            public int total { get; set; }
            public int percent { get; set; }
        }

        public class Progress
        {
            public int progress { get; set; }
            public int total { get; set; }
            public int percent { get; set; }
        }

        public class Votes
        {
            public string self { get; set; }
            public int votes { get; set; }
            public bool hasVoted { get; set; }
        }

        public class Worklog2
        {
            public int startAt { get; set; }
            public int maxResults { get; set; }
            public int total { get; set; }
            public List<object> worklogs { get; set; }
        }

        public class Issuetype3
        {
            public string self { get; set; }
            public string id { get; set; }
            public string description { get; set; }
            public string iconUrl { get; set; }
            public string name { get; set; }
            public bool subtask { get; set; }
        }

        public class Project
        {
            public string self { get; set; }
            public string id { get; set; }
            public string key { get; set; }
            public string name { get; set; }
            public AvatarUrls avatarUrls { get; set; }
        }

        public class Customfield10704
        {
            public string self { get; set; }
            public string value { get; set; }
            public string id { get; set; }
        }

        public class Customfield10705
        {
            public string self { get; set; }
            public string value { get; set; }
            public string id { get; set; }
        }

        public class Watches
        {
            public string self { get; set; }
            public int watchCount { get; set; }
            public bool isWatching { get; set; }
        }

        public class Timetracking
        {
            public string remainingEstimate { get; set; }
            public string timeSpent { get; set; }
            public int remainingEstimateSeconds { get; set; }
            public int timeSpentSeconds { get; set; }
        }

        public class Author2
        {
            public string self { get; set; }
            public string name { get; set; }
            public string key { get; set; }
            public string emailAddress { get; set; }
            public AvatarUrls avatarUrls { get; set; }
            public string displayName { get; set; }
            public bool active { get; set; }
            public string timeZone { get; set; }
        }

        public class Attachment
        {
            public string self { get; set; }
            public string id { get; set; }
            public string filename { get; set; }
            public Author2 author { get; set; }
            public DateTime created { get; set; }
            public int size { get; set; }
            public string mimeType { get; set; }
            public string content { get; set; }
            public string thumbnail { get; set; }
        }

        public class Customfield11214
        {
            public string self { get; set; }
            public string value { get; set; }
            public string id { get; set; }
        }

        public class Comment
        {
            public List<object> comments { get; set; }
            public int maxResults { get; set; }
            public int total { get; set; }
            public int startAt { get; set; }
        }

        public class Fields
        {
            public List<FixVersion> fixVersions { get; set; }
            public object customfield_11200 { get; set; }
            public object resolution { get; set; }
            public object customfield_11201 { get; set; }
            public object customfield_11202 { get; set; }
            public object customfield_11203 { get; set; }
            public string customfield_10500 { get; set; }
            public object customfield_10502 { get; set; }
            public string customfield_10903 { get; set; }
            public DateTime lastViewed { get; set; }
            public Priority priority { get; set; }
            public List<object> labels { get; set; }
            public int timeestimate { get; set; }
            public int aggregatetimeoriginalestimate { get; set; }
            public List<object> versions { get; set; }
            public List<Issuelink> issuelinks { get; set; }
            public Assignee assignee { get; set; }
            public Status2 status { get; set; }
            public List<object> components { get; set; }
            public object customfield_10600 { get; set; }
            public int customfield_10601 { get; set; }
            public int aggregatetimeestimate { get; set; }
            public Creator creator { get; set; }
            public List<Subtask> subtasks { get; set; }
            public Reporter reporter { get; set; }
            public Aggregateprogress aggregateprogress { get; set; }
            public Progress progress { get; set; }
            public Votes votes { get; set; }
            public Worklog2 worklog { get; set; }
            public Issuetype3 issuetype { get; set; }
            public int timespent { get; set; }
            public object customfield_11120 { get; set; }
            public Project project { get; set; }
            public object customfield_11121 { get; set; }
            public double customfield_11122 { get; set; }
            public double customfield_11123 { get; set; }
            public int aggregatetimespent { get; set; }
            public double customfield_11124 { get; set; }
            public double customfield_10302 { get; set; }
            public object customfield_11118 { get; set; }
            public object customfield_11119 { get; set; }
            public Customfield10704 customfield_10704 { get; set; }
            public object resolutiondate { get; set; }
            public Customfield10705 customfield_10705 { get; set; }
            public object customfield_10706 { get; set; }
            public string customfield_10707 { get; set; }
            public int workratio { get; set; }
            public object customfield_10708 { get; set; }
            public string customfield_10709 { get; set; }
            public Watches watches { get; set; }
            public DateTime created { get; set; }
            public object customfield_11110 { get; set; }
            public object customfield_11111 { get; set; }
            public object customfield_11112 { get; set; }
            public object customfield_11113 { get; set; }
            public object customfield_11114 { get; set; }
            public double customfield_10301 { get; set; }
            public object customfield_11115 { get; set; }
            public string customfield_11105 { get; set; }
            public string customfield_11106 { get; set; }
            public string customfield_11107 { get; set; }
            public string customfield_11108 { get; set; }
            public object customfield_11109 { get; set; }
            public DateTime updated { get; set; }
            public object timeoriginalestimate { get; set; }
            public string description { get; set; }
            public object customfield_11220 { get; set; }
            public object customfield_11100 { get; set; }
            public object customfield_11101 { get; set; }
            public object customfield_11102 { get; set; }
            public object customfield_11103 { get; set; }
            public Timetracking timetracking { get; set; }
            public object customfield_11104 { get; set; }
            public string customfield_10005 { get; set; }
            public object customfield_11215 { get; set; }
            public double customfield_10006 { get; set; }
            public object customfield_11216 { get; set; }
            public object customfield_10007 { get; set; }
            public object customfield_11217 { get; set; }
            public object customfield_10008 { get; set; }
            public object customfield_11218 { get; set; }
            public List<Attachment> attachment { get; set; }
            public object customfield_11219 { get; set; }
            public string summary { get; set; }
            public object customfield_10000 { get; set; }
            public object customfield_11210 { get; set; }
            public object customfield_11211 { get; set; }
            public object customfield_11212 { get; set; }
            public object customfield_11213 { get; set; }
            public object customfield_10004 { get; set; }
            public object customfield_10400 { get; set; }
            public Customfield11214 customfield_11214 { get; set; }
            public object customfield_11204 { get; set; }
            public object customfield_11205 { get; set; }
            public object customfield_11206 { get; set; }
            public object customfield_11207 { get; set; }
            public object customfield_11208 { get; set; }
            public object customfield_11209 { get; set; }
            public string duedate { get; set; }
            public Comment comment { get; set; }
        }
        
        public class Groups
        {
            public int size { get; set; }
            public List<object> items { get; set; }
        }

        public class ApplicationRoles
        {
            public int size { get; set; }
            public List<object> items { get; set; }
        }

        public class User
        {
            public string self { get; set; }
            public string key { get; set; }
            public string name { get; set; }
            public string emailAddress { get; set; }
            public AvatarUrls avatarUrls { get; set; }
            public string displayName { get; set; }
            public bool active { get; set; }
            public string timeZone { get; set; }
            public string locale { get; set; }
            public Groups groups { get; set; }
            public ApplicationRoles applicationRoles { get; set; }
            public string expand { get; set; }
        }

        public class Priority4
        {
            public string self { get; set; }
            public string iconUrl { get; set; }
            public string name { get; set; }
            public string id { get; set; }
        }
        
        public class Assignee2
        {
            public string self { get; set; }
            public string name { get; set; }
            public string key { get; set; }
            public string emailAddress { get; set; }
            public AvatarUrls avatarUrls { get; set; }
            public string displayName { get; set; }
            public bool active { get; set; }
            public string timeZone { get; set; }
        }

        public class StatusCategory4
        {
            public string self { get; set; }
            public int id { get; set; }
            public string key { get; set; }
            public string colorName { get; set; }
            public string name { get; set; }
        }

        public class Status4
        {
            public string self { get; set; }
            public string description { get; set; }
            public string iconUrl { get; set; }
            public string name { get; set; }
            public string id { get; set; }
            public StatusCategory4 statusCategory { get; set; }
        }

        public class Creator2
        {
            public string self { get; set; }
            public string name { get; set; }
            public string key { get; set; }
            public string emailAddress { get; set; }
            public AvatarUrls avatarUrls { get; set; }
            public string displayName { get; set; }
            public bool active { get; set; }
            public string timeZone { get; set; }
        }

        public class Reporter2
        {
            public string self { get; set; }
            public string name { get; set; }
            public string key { get; set; }
            public string emailAddress { get; set; }
            public AvatarUrls avatarUrls { get; set; }
            public string displayName { get; set; }
            public bool active { get; set; }
            public string timeZone { get; set; }
        }

        public class Aggregateprogress2
        {
            public int progress { get; set; }
            public int total { get; set; }
            public int? percent { get; set; }
        }

        public class Progress2
        {
            public int progress { get; set; }
            public int total { get; set; }
            public int? percent { get; set; }
        }

        public class Votes2
        {
            public string self { get; set; }
            public int votes { get; set; }
            public bool hasVoted { get; set; }
        }

        public class Worklog3
        {
            public int startAt { get; set; }
            public int maxResults { get; set; }
            public int total { get; set; }
            public List<object> worklogs { get; set; }
        }

        public class Issuetype4
        {
            public string self { get; set; }
            public string id { get; set; }
            public string description { get; set; }
            public string iconUrl { get; set; }
            public string name { get; set; }
            public bool subtask { get; set; }
        }


        public class Project2
        {
            public string self { get; set; }
            public string id { get; set; }
            public string key { get; set; }
            public string name { get; set; }
            public AvatarUrls avatarUrls { get; set; }
        }

        public class Customfield107042
        {
            public string self { get; set; }
            public string value { get; set; }
            public string id { get; set; }
        }

        public class Customfield107052
        {
            public string self { get; set; }
            public string value { get; set; }
            public string id { get; set; }
        }

        public class Customfield10706
        {
            public string self { get; set; }
            public string value { get; set; }
            public string id { get; set; }
        }

        public class Watches2
        {
            public string self { get; set; }
            public int watchCount { get; set; }
            public bool isWatching { get; set; }
        }

        public class Customfield11114
        {
            public string self { get; set; }
            public string value { get; set; }
            public string id { get; set; }
        }

        public class Timetracking2
        {
            public string remainingEstimate { get; set; }
            public string timeSpent { get; set; }
            public int? remainingEstimateSeconds { get; set; }
            public int? timeSpentSeconds { get; set; }
        }

        public class Customfield11217
        {
            public string self { get; set; }
            public string value { get; set; }
            public string id { get; set; }
        }

        public class Customfield112142
        {
            public string self { get; set; }
            public string value { get; set; }
            public string id { get; set; }
        }

        public class Comment2
        {
            public List<object> comments { get; set; }
            public int maxResults { get; set; }
            public int total { get; set; }
            public int startAt { get; set; }
        }

        public class Fields4
        {
            public List<object> fixVersions { get; set; }
            public object customfield_11200 { get; set; }
            public object resolution { get; set; }
            public object customfield_11201 { get; set; }
            public double? customfield_11202 { get; set; }
            public object customfield_11203 { get; set; }
            public string customfield_10500 { get; set; }
            public object customfield_10502 { get; set; }
            public string customfield_10903 { get; set; }
            public DateTime lastViewed { get; set; }
            public Priority4 priority { get; set; }
            public List<object> labels { get; set; }
            public int? timeestimate { get; set; }
            public int? aggregatetimeoriginalestimate { get; set; }
            public List<object> versions { get; set; }
            public List<object> issuelinks { get; set; }
            public Assignee2 assignee { get; set; }
            public Status4 status { get; set; }
            public List<object> components { get; set; }
            public double? customfield_10600 { get; set; }
            public int? customfield_10601 { get; set; }
            public int? aggregatetimeestimate { get; set; }
            public Creator2 creator { get; set; }
            public List<object> subtasks { get; set; }
            public Reporter2 reporter { get; set; }
            public Aggregateprogress2 aggregateprogress { get; set; }
            public Progress2 progress { get; set; }
            public Votes2 votes { get; set; }
            public Worklog3 worklog { get; set; }
            public Issuetype4 issuetype { get; set; }
            public int? timespent { get; set; }
            public object customfield_11120 { get; set; }
            public Project2 project { get; set; }
            public double? customfield_11121 { get; set; }
            public double customfield_11122 { get; set; }
            public double customfield_11123 { get; set; }
            public int? aggregatetimespent { get; set; }
            public double customfield_11124 { get; set; }
            public double customfield_10302 { get; set; }
            public object customfield_11118 { get; set; }
            public object customfield_11119 { get; set; }
            public Customfield107042 customfield_10704 { get; set; }
            public object resolutiondate { get; set; }
            public Customfield107052 customfield_10705 { get; set; }
            public Customfield10706 customfield_10706 { get; set; }
            public string customfield_10707 { get; set; }
            public int workratio { get; set; }
            public string customfield_10708 { get; set; }
            public string customfield_10709 { get; set; }
            public Watches2 watches { get; set; }
            public DateTime created { get; set; }
            public double? customfield_11110 { get; set; }
            public double? customfield_11111 { get; set; }
            public double? customfield_11112 { get; set; }
            public object customfield_11113 { get; set; }
            public Customfield11114 customfield_11114 { get; set; }
            public double customfield_10301 { get; set; }
            public object customfield_11115 { get; set; }
            public string customfield_11105 { get; set; }
            public string customfield_11106 { get; set; }
            public object customfield_11107 { get; set; }
            public object customfield_11108 { get; set; }
            public object customfield_11109 { get; set; }
            public DateTime updated { get; set; }
            public object timeoriginalestimate { get; set; }
            public string description { get; set; }
            public object customfield_11220 { get; set; }
            public string customfield_11100 { get; set; }
            public object customfield_11101 { get; set; }
            public string customfield_11102 { get; set; }
            public object customfield_11103 { get; set; }
            public Timetracking2 timetracking { get; set; }
            public object customfield_11104 { get; set; }
            public string customfield_10005 { get; set; }
            public double? customfield_11215 { get; set; }
            public double? customfield_10006 { get; set; }
            public object customfield_11216 { get; set; }
            public object customfield_10007 { get; set; }
            public Customfield11217 customfield_11217 { get; set; }
            public object customfield_10008 { get; set; }
            public object customfield_11218 { get; set; }
            public List<object> attachment { get; set; }
            public object customfield_11219 { get; set; }
            public string summary { get; set; }
            public object customfield_10000 { get; set; }
            public object customfield_11210 { get; set; }
            public object customfield_11211 { get; set; }
            public object customfield_11212 { get; set; }
            public object customfield_11213 { get; set; }
            public object customfield_10004 { get; set; }
            public int? customfield_10400 { get; set; }
            public Customfield112142 customfield_11214 { get; set; }
            public object customfield_11204 { get; set; }
            public object customfield_11205 { get; set; }
            public object customfield_11206 { get; set; }
            public object customfield_11207 { get; set; }
            public object customfield_11208 { get; set; }
            public object customfield_11209 { get; set; }
            public string duedate { get; set; }
            public Comment2 comment { get; set; }
        }

        public class Issue
        {
            public string expand { get; set; }
            public string id { get; set; }
            public string self { get; set; }
            public string key { get; set; }
            public Fields4 fields { get; set; }
        }

        public class Item
        {
            public string field { get; set; }
            public string fieldtype { get; set; }
            public string fieldId { get; set; }
            public string from { get; set; }
            public string fromString { get; set; }
            public string to { get; set; }
            public string toString { get; set; }
        }

        public class Changelog
        {
            public string id { get; set; }
            public List<Item> items { get; set; }
        }

        public class ObjetoRecebido
        {
            public object timestamp { get; set; }
            public string webhookEvent { get; set; }
            public Worklog worklog { get; set; }
            public string expand { get; set; }
            public string id { get; set; }
            public string self { get; set; }
            public string key { get; set; }
            public Fields fields { get; set; }
            public string issue_event_type_name { get; set; }
            public User user { get; set; }
            public Issue issue { get; set; }
            public Changelog changelog { get; set; }
        }

        public class Customfield10219
        {
            public string name { get; set; }
        }

        public class Fields5
        {
            public Customfield10219 customfield_10219 { get; set; }
        }

        public class Transition
        {
            public int id { get; set; }
        }

        public class ObjetoRetornado
        {
            public double timeSpentSeconds { get; set; }
            public Fields5 fields { get; set; }
            public Transition transition { get; set; }
            public string codigoExterno { get; set; }
            public int? statusId { get; set; }
        }

        public class Author3
        {
            public string self { get; set; }
            public string name { get; set; }
            public string key { get; set; }
            public string emailAddress { get; set; }
            public AvatarUrls avatarUrls { get; set; }
            public string displayName { get; set; }
            public bool active { get; set; }
            public string timeZone { get; set; }
        }
        
        public class UpdateAuthor2
        {
            public string self { get; set; }
            public string name { get; set; }
            public string key { get; set; }
            public string emailAddress { get; set; }
            public AvatarUrls avatarUrls { get; set; }
            public string displayName { get; set; }
            public bool active { get; set; }
            public string timeZone { get; set; }
        }

        public class JsonSerializer
        {
            public object DateFormat { get; set; }
            public object RootElement { get; set; }
            public object Namespace { get; set; }
            public string ContentType { get; set; }
        }

        public class XmlSerializer
        {
            public object RootElement { get; set; }
            public object Namespace { get; set; }
            public object DateFormat { get; set; }
            public string ContentType { get; set; }
        }

        public class Parameter
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public int Type { get; set; }
            public object ContentType { get; set; }
        }

        public class Method
        {
            public string Name { get; set; }
            public string AssemblyName { get; set; }
            public string ClassName { get; set; }
            public string Signature { get; set; }
            public string Signature2 { get; set; }
            public int MemberType { get; set; }
            public object GenericArguments { get; set; }
        }

        public class Target
        {
        }

        public class OnBeforeDeserialization
        {
            public Method Method { get; set; }
            public Target Target { get; set; }
        }

        public class Request
        {
            public object UserState { get; set; }
            public List<int> AllowedDecompressionMethods { get; set; }
            public bool AlwaysMultipartFormData { get; set; }
            public JsonSerializer JsonSerializer { get; set; }
            public XmlSerializer XmlSerializer { get; set; }
            public object ResponseWriter { get; set; }
            public bool UseDefaultCredentials { get; set; }
            public List<Parameter> Parameters { get; set; }
            public List<object> Files { get; set; }
            public int Method { get; set; }
            public string Resource { get; set; }
            public int RequestFormat { get; set; }
            public object RootElement { get; set; }
            public OnBeforeDeserialization OnBeforeDeserialization { get; set; }
            public object DateFormat { get; set; }
            public object XmlNamespace { get; set; }
            public object Credentials { get; set; }
            public int Timeout { get; set; }
            public int ReadWriteTimeout { get; set; }
            public int Attempts { get; set; }
        }

        public class Cooky
        {
            public string Comment { get; set; }
            public object CommentUri { get; set; }
            public bool Discard { get; set; }
            public string Domain { get; set; }
            public bool Expired { get; set; }
            public DateTime Expires { get; set; }
            public bool HttpOnly { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }
            public string Port { get; set; }
            public bool Secure { get; set; }
            public DateTime TimeStamp { get; set; }
            public string Value { get; set; }
            public int Version { get; set; }
        }

        public class Header
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public int Type { get; set; }
            public object ContentType { get; set; }
        }

        public class ProtocolVersion
        {
            public int Major { get; set; }
            public int Minor { get; set; }
            public int Build { get; set; }
            public int Revision { get; set; }
            public int MajorRevision { get; set; }
            public int MinorRevision { get; set; }
        }

        public class Transition2
        {
            public int id { get; set; }
        }

        public class Errors
        {
            public string timeLogged { get; set; }
        }

        public class ResponseServer
        {
            public string self { get; set; }
            public Author3 author { get; set; }
            public UpdateAuthor2 updateAuthor { get; set; }
            public DateTime created { get; set; }
            public DateTime updated { get; set; }
            public DateTime started { get; set; }
            public string timeSpent { get; set; }
            public int timeSpentSeconds { get; set; }
            public string id { get; set; }
            public string issueId { get; set; }
            public Request Request { get; set; }
            public string ContentType { get; set; }
            public int? ContentLength { get; set; }
            public string ContentEncoding { get; set; }
            public string Content { get; set; }
            public int? StatusCode { get; set; }
            public bool? IsSuccessful { get; set; }
            public string StatusDescription { get; set; }
            public string RawBytes { get; set; }
            public string ResponseUri { get; set; }
            public string Server { get; set; }
            public List<Cooky> Cookies { get; set; }
            public List<Header> Headers { get; set; }
            public int? ResponseStatus { get; set; }
            public object ErrorMessage { get; set; }
            public object ErrorException { get; set; }
            public ProtocolVersion ProtocolVersion { get; set; }
            public Transition2 transition { get; set; }
            public string codigoExterno { get; set; }
            public int? statusId { get; set; }
            public List<string> errorMessages { get; set; }
            public Errors errors { get; set; }
        }
        public string data { get; set; }
        public string evento { get; set; }
        public ObjetoRecebido objetoRecebido { get; set; }
        public ObjetoRetornado objetoRetornado { get; set; }
        public ResponseServer responseServer { get; set; }
    }
}