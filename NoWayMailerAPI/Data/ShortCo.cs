namespace NoWayMailerAPI.Data;

public class ShortCo
{
	public bool ok { get; set; }
	public Result result { get; set; }
}

public class Result
{
	public string code { get; set; }
	public string short_link { get; set; }
	public string full_short_link { get; set; }
	public string short_link2 { get; set; }
	public string full_short_link2 { get; set; }
	public string short_link3 { get; set; }
	public string full_short_link3 { get; set; }
	public string share_link { get; set; }
	public string full_share_link { get; set; }
	public string original_link { get; set; }
}

