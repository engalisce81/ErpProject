using ErpProject.CreatSteps;
using ErpProject.Models;

namespace ErpProject.Service
{
    public class ConvertStream
    {
        public static StepImage ConvertFromStreamToFormFile(Image image)
        {
            MemoryStream stream = new MemoryStream(image.Data);
            FormFile formFile = new FormFile(stream, 0, image.Data.Length, image.FileName, image.FileName);
            StepImage stepImage = new StepImage() { Id = image.Id, FormFile = formFile };
            return stepImage;
        }
    }
}
