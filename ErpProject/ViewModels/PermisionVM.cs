namespace ErpProject.ViewModels
{
    public class PermisionVM
    {
        public string RoleId {  get; set; }
        public string RoleName {  get; set; }
        public List<CheckBoxVM> checkBoxVMs { get; set; }
    }
}
