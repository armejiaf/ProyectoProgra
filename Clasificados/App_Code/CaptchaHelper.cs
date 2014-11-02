using BotDetect.Web.UI.Mvc;

namespace Clasificados
{
    public class CaptchaHelper
    {
        public static MvcCaptcha GetSampleCaptcha()
        {
            // create the control instance
            var sampleCaptcha = new MvcCaptcha("SampleCaptcha") {UserInputClientID = "CaptchaCode"};

            return sampleCaptcha;
        }
    }
}