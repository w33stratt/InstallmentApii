using Microsoft.AspNetCore.Mvc;

namespace InstallmentAPI.Controllers
{
    [ApiController]
    [Route("api/installments")]
    public class InstallmentController : ControllerBase
    {
        [HttpPost]
        public IActionResult CalculateInstallment([FromBody] InstallmentRequest request)
        {
            if (request.OTR <= 0 || request.DownPayment <= 0 || request.JangkaWaktu <= 0)
            {
                return BadRequest("Invalid input values.");
            }

            decimal pokokUtang = request.OTR - request.DownPayment;
            decimal bunga = request.JangkaWaktu <= 12 ? 0.12m : (request.JangkaWaktu <= 24 ? 0.14m : 0.165m);
            decimal totalBunga = pokokUtang * bunga;
            decimal angsuranPerBulan = (pokokUtang + totalBunga) / request.JangkaWaktu;

            var schedule = new List<InstallmentSchedule>();
            for (int i = 1; i <= request.JangkaWaktu; i++)
            {
                schedule.Add(new InstallmentSchedule
                {
                    AngsuranKe = i,
                    AngsuranPerBulan = Math.Round(angsuranPerBulan, 2),
                    TanggalJatuhTempo = DateTime.Now.AddMonths(i).ToString("yyyy-MM-dd")
                });
            }

            return Ok(schedule);
        }
    }

    public class InstallmentRequest
    {
        public decimal OTR { get; set; }
        public decimal DownPayment { get; set; }
        public int JangkaWaktu { get; set; } 
    }

    public class InstallmentSchedule
    {
        public int AngsuranKe { get; set; }
        public decimal AngsuranPerBulan { get; set; }
        public string? TanggalJatuhTempo { get; set; }
    }
}
