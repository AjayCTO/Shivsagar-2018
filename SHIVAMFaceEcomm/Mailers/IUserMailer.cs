using Mvc.Mailer;

namespace SHIVAMFaceEcomm.Mailers
{ 
    public interface IUserMailer
    {
			MvcMailMessage Welcome();
			MvcMailMessage PasswordReset();
	}
}