namespace ShoppingCartWeb
{
    public class CustomSelectListItem
    {
        public string Text { get; set; }
        public int Value { get; set; }
        public int MenuRoleMappingId { get; set; }
        public int MenuId { get; set; }
        public int RoleId { get; set; }
        public string MenuName { get; set; }
        public int ParentId { get; set; }
        public bool Selected { get; set; }
    }
}
