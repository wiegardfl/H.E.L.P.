namespace HELP.DataModels
{
    public class User
    {
        #region Constructors
        public User() {}
        #endregion

        #region Properties
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        #endregion
    }
}