using System.Net.Mail;
using System.Net;

namespace SU24_VMO_API.Supporters.EmailSupporter
{
    public class EmailSupporter
    {
        private static string subjectForForgetPass = "Xác thực tài khoản: Yêu cầu Đặt lại Mật khẩu";
        private static string subjectForSuccessfulDonate = "Bạn vừa ủng hộ thành công!";


        public static void SendEmail(string email, string subject, string body)
        {
            // send otp
            using (MailMessage mm = new MailMessage("vmoautomailer@gmail.com", email))
            {
                mm.Subject = subject;

                mm.Body = body;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential("vmoautomailer@gmail.com", "npufojpvlcxiowda");
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
            }
        }

        public static string? SendEmailForResetPassword(string email)
        {
            // send otp
            using (MailMessage mm = new MailMessage("vmoautomailer@gmail.com", email))
            {
                //create random number
                Random random = new Random();
                string digits = string.Empty;
                for (int i = 0; i < 6; i++)
                {
                    digits += random.Next(0, 10).ToString();
                };
                var bodyForForgetPass = $"Xin chào!\r\n\r\nChúng tôi đã nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn. Để tiến hành đặt lại mật khẩu, vui lòng sử dụng mã xác minh dưới đây:\r\n\r\nMã xác minh: {digits}\r\n\r\nVui lòng nhập mã này vào trang đặt lại mật khẩu để tiếp tục quy trình. Nếu bạn không yêu cầu đặt lại mật khẩu, xin vui lòng bỏ qua email này hoặc liên hệ với chúng tôi để được hỗ trợ.\r\n\r\nXin cảm ơn,\r\nĐội ngũ hỗ trợ khách hàng";
                mm.Subject = subjectForForgetPass;

                mm.Body = bodyForForgetPass;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                NetworkCredential NetworkCred = new NetworkCredential("vmoautomailer@gmail.com", "rwmplewvbcisefuz");
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
                return digits;
            }
        }


        public static void SendEmailWithSuccessDonate(string email, string fullname, string campaignName, float amountDonate, DateTime donateTime)
        {
            using (MailMessage mm = new MailMessage("vmoautomailer@gmail.com", email))
            {
                mm.Subject = subjectForSuccessfulDonate;
                var currentTime = donateTime.ToString("HH:mm:ss dd-MM-yyyy");
                string formattedAmount = amountDonate.ToString("N0") + " đồng";
                string body = $@"
            <p>Xin chào <b>{fullname}</b></p>
            <p>Cảm ơn bạn đã đồng hành cùng chúng tôi thông qua nền tảng VMO.</p>
            <p>Thông tin ủng hộ của bạn như sau:</p>
            <ul>
                <li>Chiến dịch ủng hộ: <b>{campaignName}</b></li>
                <li>Số tiền ủng hộ: <b>{formattedAmount}</b></li>
                <li>Thời gian ủng hộ: <b>{currentTime}</b></li>
                <li>Tài khoản nhận ủng hộ: <b>0946517841</b></li>
                <li>Chủ tài khoản: <b>CHAU NHAT TRUONG</b></li>
            </ul>
            <p>Bạn có thể theo dõi danh sách ủng hộ và các thông tin cập nhật về chiến dịch này <a href='#'>[đường dẫn đến chiến dịch đó]</a></p>
            <p>Chúc bạn hạnh phúc và thành công! Hãy đồng hành cùng chúng tôi!</p>
        ";
                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential("vmoautomailer@gmail.com", "rwmplewvbcisefuz");
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
            }
        }
    }
}
