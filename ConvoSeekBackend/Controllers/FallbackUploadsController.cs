using Microsoft.AspNetCore.Mvc;

namespace ConvoSeekBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FallbackUploadsController : ControllerBase
    {
        public async Task<IActionResult> Post(IFormFile file)
        {
            return Ok();
        }

        // json mode use gpt-3.5-turbo-0125, response_format json_object // not great output
        /*
         * system_prompt = "You are an assistant that only returns JSON object with the requested information"


prompt = "Please return a JSON object ..."

# Turn it into a function that will tell us about anyone!
def get_person(name):
    prompt = f"Please return a JSON object about {name}. " + \
             "Include full name, birth date, date of death (if applicable), " + \
             "and list of accomplishments."
             
    completion = complete(prompt)

    # we could just print the output here, but let's use this
    # to confirm it's an actual JSON
    j = json.loads(get_response(completion))
    return j

prompt = f"""Please pull all the important information from \
the below quarterly earnings report, enclosed in \
triple backticks. Return the result as a JSON.\n\n\
{report}

"""

        */
    }
}
