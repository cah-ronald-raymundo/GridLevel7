namespace NetAPIGrid.Models
{
    public class vw_ActiveWindow_Idle_With_Duration
    {
        public int Id { get; set; }
        public string? EID { get; set; }
        public Int16 TeamId { get; set; }
        public int DeptId { get; set; }
        public byte ProjectId { get; set; }
        public string? EventType { get; set; }
        public string? Details { get; set; }
        public DateTime TimeStamp { get; set; }
        public int Duration { get; set; }
        public string? Activity_Type { get; set; }
        public int Year { get; set; }
        public int Month_Of_Year { get; set; }
        public int Week_Of_Month { get; set; }
        public int Day_Of_Week { get; set; }
        public int Day_Of_Month { get; set; }
        public int Hour_Of_Day { get; set; }
    }
}
