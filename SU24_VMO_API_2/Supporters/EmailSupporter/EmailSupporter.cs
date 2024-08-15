using System.Net.Mail;
using System.Net;
using BusinessObject.Models;
using System.Net.Sockets;
using System.Text;
using DnsClient;
using System;

namespace SU24_VMO_API.Supporters.EmailSupporter
{
    public class EmailSupporter
    {
        private static string subjectForForgetPass = "Xác thực tài khoản: Yêu cầu Đặt lại Mật khẩu";
        private static string subjectForSuccessfulDonate = "Bạn vừa ủng hộ thành công!";
        private static string subjectForCreateNewAccount = "Xác nhận tạo tài khoản!";
        private static string subjectForBanCampaign = "Báo cáo về Chiến dịch Không Minh bạch!";




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
                var bodyForForgetPass = $"Xin chào!\r\n\r\nChúng tôi đã nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn. Để tiến hành đặt lại mật khẩu, vui lòng sử dụng mã xác minh dưới đây:\r\n\r\nMã xác minh: {digits}\r\n\r\nVui lòng nhập mã này vào trang đặt lại mật khẩu để tiếp tục quy trình. Nếu bạn không yêu cầu đặt lại mật khẩu, xin vui lòng bỏ qua email này hoặc liên hệ với chúng tôi để được hỗ trợ.\r\n\r\nXin cảm ơn,\r\nĐội ngũ hỗ trợ người dùng";
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


        public static void SendEmailForReportCampaign(string email, string campaignName, string username, DateTime startDate, DateTime endDate)
        {
            using (MailMessage mm = new MailMessage("vmoautomailer@gmail.com", email))
            {
                mm.Subject = subjectForBanCampaign;
                var startDateFormat = startDate.ToString("HH:mm:ss dd-MM-yyyy");
                var endDateFormat = endDate.ToString("HH:mm:ss dd-MM-yyyy");
                var delayTime = TimeHelper.TimeHelper.GetTime(DateTime.UtcNow).ToString("HH:mm:ss dd-MM-yyyy");
                

                    string body = $@"
            <p>Kính gửi <b>{username}</b></p>
            <p>Chúng tôi rất tiếc khi một chiến dịch gần đây mà chúng tôi nhận thấy có dấu hiệu không minh bạch. Chi tiết của chiến dịch như sau:</p>
            <ul>
                <li>Tên chiến dịch: <b>{campaignName}</b></li>
                <li>Ngày bắt đầu: <b>{startDateFormat}</b></li>
                <li>Ngày kết thúc: <b>{endDateFormat}</b></li>
                <li>Ngày tạm ngưng: <b>{delayTime}</b></li>
            </ul>
            <p>Chúng tôi nhận thấy rằng các hành động và thông tin trong chiến dịch này không hoàn toàn minh bạch và rõ ràng, điều này có thể gây ảnh hưởng xấu đến tổ chức và uy tín của chúng tôi. Chúng tôi hiện tại đang kiểm tra và xử lý vấn đề này kịp thời để đảm bảo tính minh bạch và uy tín của các chiến dịch trong tương lai.</p>
            <p>Chúng tôi xin chân thành xin lỗi quý vị và sẽ đưa ra hướng giải quyết cho vấn đề này. Xin chân thành cảm ơn!</p>
            <p>Bạn có thể theo dõi danh sách ủng hộ và các thông tin cập nhật về các chiến dịch khác <a href='https://su24-vmo-fe.vercel.app/viewCampaigns'>Tại đây</a></p>
            <p>Chúc bạn hạnh phúc và thành công! Hãy đồng hành cùng chúng tôi!</p>
            <p>Hệ thống VMO</p>
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

        public async static Task<string?> SendOTPForCreateNewUser(string email)
        {
            //var a = await VerifyEmailAsync(email);
            try
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
                    var bodyForCreateNew = $"Xin chào!\r\n\r\nChúng tôi đã nhận được yêu cầu tạo tài khoản của bạn. Để tiến hành các bước kế tiếp, vui lòng sử dụng mã xác minh dưới đây:\r\n\r\nMã xác minh: {digits}\r\n\r\nVui lòng nhập mã này ở trang đăng kí để tiếp tục quy trình. Nếu bạn không yêu cầu đặt lại mật khẩu, xin vui lòng bỏ qua email này hoặc liên hệ với chúng tôi để được hỗ trợ.\r\n\r\nXin cảm ơn,\r\nĐội ngũ hỗ trợ người dùng";
                    mm.Subject = subjectForCreateNewAccount;

                    mm.Body = bodyForCreateNew;
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
            catch (Exception ex)
            {
                // Log the exception (ex) or handle it accordingly
                throw new Exception("Error sending email: " + ex.Message);
            }
        }


        public static void SendEmailWithSuccessDonateForCampaignTierI(string email, string fullname, string campaignName, float amountDonate, DateTime donateTime, Guid campaignId)
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
            <p>Bạn có thể theo dõi danh sách ủng hộ và các thông tin cập nhật về chiến dịch này <a href='https://su24-vmo-fe.vercel.app/viewCampaigns/campaignDetail/tier1/{campaignId}'>Tại đây</a></p>
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

        public static void SendEmailWithSuccessDonateForCampaignTierII(string email, string fullname, string campaignName, float amountDonate, DateTime donateTime, Guid campaignId)
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
            <p>Bạn có thể theo dõi danh sách ủng hộ và các thông tin cập nhật về chiến dịch này <a href='https://su24-vmo-fe.vercel.app/viewCampaigns/campaignDetail/tier2/{campaignId}'>Tại đây</a></p>
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


        public static async Task<bool> VerifyEmailAsync(string email)
        {
            try
            {
                var domain = email.Split('@')[1];
                var lookup = new LookupClient();
                var result = await lookup.QueryAsync(domain, QueryType.MX);
                var mailExchangers = result.Answers.MxRecords().OrderBy(priority => priority.Preference);

                foreach (var mx in mailExchangers)
                {
                    using (var tcpClient = new TcpClient())
                    {
                        await tcpClient.ConnectAsync(mx.Exchange.ToString(), 25);
                        using (var networkStream = tcpClient.GetStream())
                        {
                            var reader = new System.IO.StreamReader(networkStream, Encoding.ASCII);
                            var writer = new System.IO.StreamWriter(networkStream, Encoding.ASCII) { AutoFlush = true };

                            if (await CheckSmtpResponseAsync(reader, writer, "HELO gmail.com"))
                            {
                                if (await CheckSmtpResponseAsync(reader, writer, $"MAIL FROM:<vmoautomailer@gmail.com>"))
                                {
                                    if (await CheckSmtpResponseAsync(reader, writer, $"RCPT TO:<{email}>"))
                                    {
                                        await CheckSmtpResponseAsync(reader, writer, "QUIT");
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Handle exceptions appropriately
            }

            return false;
        }

        private static async Task<bool> CheckSmtpResponseAsync(System.IO.StreamReader reader, System.IO.StreamWriter writer, string command)
        {
            await writer.WriteLineAsync(command);
            string response = await reader.ReadLineAsync();
            return response.StartsWith("250");
        }
    }
}
