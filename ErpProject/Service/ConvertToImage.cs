using ErpProject.CreatSteps;
using ErpProject.Models;

namespace ErpProject.Service
{
    public class ConvertToImage
    {
        public static async Task AsignImage(StepImage stepImage, Image _image)
        {
            using (var memoryStream = new MemoryStream())
            {
                await stepImage.FormFile.CopyToAsync(memoryStream);
                _image.Id = stepImage.Id;
                _image.Data = memoryStream.ToArray();
                _image.FileName = stepImage.FormFile.FileName;
                _image.ContentType = stepImage.FormFile.ContentType;
                _image.Accepted = true;
                _image.ProductId=stepImage.RelationId;
            }
        }
    }
}
