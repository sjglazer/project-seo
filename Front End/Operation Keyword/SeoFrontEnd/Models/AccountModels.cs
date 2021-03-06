﻿using SeoFrontEnd.ServiceReference1;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;


namespace SeoFrontEnd.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
    }


    public class WebsiteModel
    {
        public string url { get; set; }

        public List<string> urls { get; set; }

        public int urlLimit { get; set; }
    }

    public class KeywordModel
    {
        public List<string> urls { get; set; }

        public string keywords { get; set; }

        public int selectedIndex { get; set; }
    }


    public class UrlKeywordSet
    {
        
        // Copy constructor. 
        public UrlKeywordSet(ServiceReference1.UrlKeywordSet set)
        {
            url = set.url;
            if (set.keywords != null) keywords = new List<string>(set.keywords);
        }

        public ServiceReference1.UrlKeywordSet GetSet()
        {
            ServiceReference1.UrlKeywordSet set = new ServiceReference1.UrlKeywordSet();
            if (this.keywords != null) set.keywords = this.keywords.ToArray();
            set.url = this.url;
            return set;
        }

        public UrlKeywordSet() { }
       
        public List<string> keywords { get; set; }

        public string url { get; set; }

        public int urlLimit { get; set; }
    }

    public class UserWebsiteModel
    {
        
        [Display(Name = "url")]
        public string url { get; set; }

        [Display(Name = "keywords")]
        public List<string> keywords { get; set; }

        public string Numkeywords { get; set; }
    }

    public class KeywordStatModel
    {
        //public Dictionary<string, Dictionary<string, ServiceReference1.KeywordStat[]>> stats { get; set; }
        public Dictionary<string, Dictionary<string, KeywordStat[]>> stats { get; set; }
    }
    
    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
