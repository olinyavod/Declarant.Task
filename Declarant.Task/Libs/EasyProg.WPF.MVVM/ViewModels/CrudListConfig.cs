namespace EasyProg.WPF.MVVM.ViewModels
{
	public class CrudListConfig
	{
		public CrudListConfig()
		{
			
		}

		public CrudListConfig(string defaultTitle, string defaultEditorViewName)
		{
			DefaultTitle = defaultTitle;
			AddEditorViewName = defaultEditorViewName;
			DetailsEditorViewName = defaultEditorViewName;
		}

		public string DefaultTitle { get; set; }

		public string DetailsEditorViewName { get; set; }

		public string AddEditorViewName { get; set; }

		public string DetailsTitle { get; set; }

		public string AddTitle { get; set; }
	}
}