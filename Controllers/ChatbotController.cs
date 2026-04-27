using Microsoft.AspNetCore.Mvc;

namespace MentorDashboardApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        [HttpPost("Ask")]
        public IActionResult Ask([FromBody] ChatRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Question))
            {
                return Ok(new { answer = "عذراً، لم أفهم سؤالك. يرجى المحاولة مرة أخرى." });
            }

            string q = request.Question.ToLowerInvariant().Trim();
            string answer = GetSimulatedAIResponse(q);

            return Ok(new { answer = answer });
        }

        private string GetSimulatedAIResponse(string question)
        {
            // Simple rule-based AI simulation for the showcase
            
            if (question.Contains("سعر") || question.Contains("اسعار") || question.Contains("أسعار") || question.Contains("بكام") || question.Contains("price") || question.Contains("cost"))
            {
                return "أسعار كورساتنا تختلف حسب التخصص. يمكنك زيارة صفحة 'الكورسات' لمعرفة سعر كل كورس بالتفصيل، وغالباً ما نوفر خصومات تصل إلى 50%!";
            }
            
            if (question.Contains("شهادة") || question.Contains("شهادات") || question.Contains("معتمدة") || question.Contains("certificate"))
            {
                return "نعم بكل تأكيد! ستحصل على شهادة إتمام معتمدة من المنصة فور إنهائك لنسبة 100% من محتوى الكورس واجتيازك للاختبارات إن وجدت.";
            }

            if (question.Contains("كيف اشترك") || question.Contains("تسجيل") || question.Contains("الاشتراك") || question.Contains("تسجيل الدخول") || question.Contains("register") || question.Contains("enroll"))
            {
                return "للاشتراك، قم بالضغط على زر 'تسجيل الدخول' أو 'إنشاء حساب' من القائمة العلوية. بعد تفعيل حسابك، يمكنك اختيار الكورس والضغط على 'اشترك الآن'.";
            }

            if (question.Contains("مدة") || question.Contains("وقت") || question.Contains("طويل") || question.Contains("duration") || question.Contains("time"))
            {
                return "المدة تعتمد على الكورس نفسه. بعض الكورسات تستغرق 5 ساعات والبعض الآخر يصل إلى 50 ساعة. بمجرد شرائك للكورس، يمكنك مشاهدته في أي وقت وبدون تاريخ انتهاء (مدى الحياة).";
            }

            if (question.Contains("مرحبا") || question.Contains("هلا") || question.Contains("السلام عليكم") || question.Contains("hello") || question.Contains("hi"))
            {
                return "أهلاً بك في منصة Mentor! أنا المساعد الذكي، كيف يمكنني مساعدتك اليوم؟";
            }

            if (question.Contains("مين انت") || question.Contains("من انت") || question.Contains("ذكاء اصطناعي") || question.Contains("who are you"))
            {
                return "أنا المساعد الذكي لمنصة Mentor، تم تصميمي باستخدام الذكاء الاصطناعي للإجابة على جميع استفساراتك فوراً على مدار 24 ساعة.";
            }

            // Default Fallback Response
            return "عذراً، لم أتمكن من فهم طلبك بالكامل. يمكنك تصفح صفحة 'الكورسات' لمعرفة المزيد، أو التواصل مع الدعم الفني عبر صفحة 'تواصل معنا'.";
        }
    }

    public class ChatRequest
    {
        public string Question { get; set; } = string.Empty;
    }
}
