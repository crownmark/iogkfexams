using IOGKFExams.Server.Models.Sms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace IOGKFExams.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SmsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmsController> _logger;

        public SmsController(
            IConfiguration configuration,
            ILogger<SmsController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("send")]
        public async Task<ActionResult<SendSmsResponse>> Send(
            [FromBody] SendSmsRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.To))
            {
                return BadRequest(new SendSmsResponse
                {
                    Success = false,
                    ErrorMessage = "A destination phone number is required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new SendSmsResponse
                {
                    Success = false,
                    ErrorMessage = "A message is required."
                });
            }

            try
            {
                var accountSid =
                    _configuration["Twilio:AccountSid"];

                var authToken =
                    _configuration["Twilio:AuthToken"];

                var fromPhoneNumber =
                    _configuration["Twilio:FromPhoneNumber"];

                if (string.IsNullOrWhiteSpace(accountSid) ||
                    string.IsNullOrWhiteSpace(authToken) ||
                    string.IsNullOrWhiteSpace(fromPhoneNumber))
                {
                    _logger.LogError(
                        "Twilio configuration is incomplete.");

                    return StatusCode(500, new SendSmsResponse
                    {
                        Success = false,
                        ErrorMessage = "SMS service is not configured."
                    });
                }

                TwilioClient.Init(
                    accountSid,
                    authToken);

                var message = await MessageResource.CreateAsync(
                    body: request.Message,
                    from: new PhoneNumber(fromPhoneNumber),
                    to: new PhoneNumber(request.To));

                return Ok(new SendSmsResponse
                {
                    Success = true,
                    MessageSid = message.Sid,
                    Status = message.Status?.ToString()
                });
            }
            catch (ApiException ex)
            {
                _logger.LogError(
                    ex,
                    "Twilio API error sending SMS to {PhoneNumber}",
                    request.To);

                return BadRequest(new SendSmsResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error sending SMS to {PhoneNumber}",
                    request.To);

                return StatusCode(500, new SendSmsResponse
                {
                    Success = false,
                    ErrorMessage = "Unable to send SMS message."
                });
            }
        }
    }
}