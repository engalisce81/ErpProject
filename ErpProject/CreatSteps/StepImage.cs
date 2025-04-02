using System.ComponentModel.DataAnnotations;

namespace ErpProject.CreatSteps
{
    public class StepImage
    {
        public int Id { get; set; }
        public int RelationId {  get; set; }
        [Required] 
        public IFormFile FormFile { get; set; }
    }
}
