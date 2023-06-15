namespace SmallClientBusiness.Common.Exceptions
{
    [Serializable]
    public class FailedLoadAvatarException: Exception
    {
        public FailedLoadAvatarException()
        {
        }

        public FailedLoadAvatarException(string message) : base(message)
        {
        }

        public FailedLoadAvatarException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}