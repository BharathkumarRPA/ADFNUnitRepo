using NUnit.Framework;
using System.Threading.Tasks;

namespace SQLtoBlob
{
    public class GivenCarDB 
    {
        private PLHelper _helper;

        [SetUp]
        public async Task WhenPipelineIsRun()
        {
            _helper = new PLHelper();
            await _helper.RunPipeline();
        }

        [Test]
        public void ThenPipelineOutcomeIsSucceeded()
        {
            Assert.AreEqual("Succeeded", _helper.PipelineOutcome);
        }
    }
}