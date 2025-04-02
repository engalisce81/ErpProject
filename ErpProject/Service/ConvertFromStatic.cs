using ErpProject.Models;

namespace ErpProject.Service
{
    public  class ConvertFromStatic
    {
        public static Image Convert(Image _image)
        {
            Image image = new Image();
            image.Accepted = _image.Accepted;
            image.ContentType = _image.ContentType;
            image.Data = _image.Data;
            image.FileName = _image.FileName;
            image.Id = _image.Id;
            return image;
        }

        public static Supplier Convert(Supplier _supplier) 
        {
            Supplier supplier = new Supplier();
            supplier.Id = _supplier.Id;
            supplier.Name = _supplier.Name;
            supplier.Email = _supplier.Email;
            supplier.Description = _supplier.Description;
            supplier.Summary = _supplier.Summary;
            supplier.Phone = _supplier.Phone;
            supplier.Address = _supplier.Address;
            supplier.Comment = _supplier.Comment;
            supplier.AcceptData = _supplier.AcceptData;
            supplier.ImageId = _supplier.ImageId;
            supplier.Purchase= _supplier.Purchase;
            return supplier;
        }
    }
}
