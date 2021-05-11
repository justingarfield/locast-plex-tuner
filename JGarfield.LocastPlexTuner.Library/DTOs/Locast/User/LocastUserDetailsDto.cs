namespace JGarfield.LocastPlexTuner.Library.DTOs.Locast
{
    public class LocastUserDetailsDto
    {
        public long created { get; set; }

        public bool didDonate { get; set; }

        public long donationExpire { get; set; }

        public string email { get; set; }

        public bool emailConfirmed { get; set; }

        public bool emailOptOut { get; set; }

        public string facebook { get; set; }

        public long id { get; set; }

        public int idrole { get; set; }

        public int lastDmaUsed { get; set; }

        public long lastDonation { get; set; }

        public double? lastDonationAmount { get; set; }

        public long lastPlayEvent { get; set; }

        public long lastlogin { get; set; }

        public string locale { get; set; }

        public long? locastCaresApplicationDate { get; set; }

        public string name { get; set; }

        public long? parentIdUser { get; set; }

        public string password { get; set; }

        public string realname { get; set; }

        public string signupSource { get; set; }

        public string subscriptionProvider { get; set; }

        public double? totalDonations { get; set; }
    }
}
