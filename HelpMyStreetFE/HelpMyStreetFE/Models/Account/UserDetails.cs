﻿namespace HelpMyStreetFE.Models.Account
{
    public class UserDetails //I expect this class to be depricated when we know how to differentiate users
    {
        public UserDetails(
            string initials, 
            string displayName, 
            string firstName, 
            string lastName,
            string emailAddress,
            string address,
            string streetChampion,
            string mobileNumber,
            string otherNumber,
            string dateOfBirth,
            string gender,
            string underlyingMedicalConditions
            )
        {
            Initials = initials;
            DisplayName = displayName;
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
            MobileNumber = mobileNumber;
            OtherNumber = otherNumber;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            Address = address;
            StreetChampion = streetChampion;
            UnderlyingMedicalConditions = underlyingMedicalConditions;
        }

        public string Initials { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public string MobileNumber { get; set; }
        public string OtherNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string UnderlyingMedicalConditions { get; set; }
        public string StreetChampion { get; set; }

    }
}
