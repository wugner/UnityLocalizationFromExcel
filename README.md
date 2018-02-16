# UnityLocalizationFromExcel
A localization tool for Unity3D based on excel documents With convenient editor selection tool and ui preview.

Below are the main features.
- Import vocabulary from excel files, such as xlsx, csv, and xml files which can be exported by Excel. 
- Helpful selection and preview in ui editing based on ugui.
- A constant C# source file will be generated to help coding when you want to control the text at runtime in your script.
- "Image Text" support. Similar to text, image can also be switched when switching to another language.
- Customizable editor vocabulary importer, rumtime vocablary, manager runtime sprite manager, and runtime font manager.

## Quick Start
1. First of all, create vocabulary files. You can find the sample format files in 
	> /Assets/Wugner/Localization/Samples/.  
	
	The first row is the header row and the headers's names are fixed, although you can change the order, and add more languages or reduce languages.
	
2. Find the config asset by clicking the menu "Localization/Config" or manually locate it in 
	> /Assets/Wugner/Localization/Generated/Resources/LocalizationConfig.asset.  
	
	You can add the files by drag them into "Localiza Excel Files" field if they are in your unity project, or add relative path to "**Localize Excel File Paths**" field.  
	When you use the relative path, you can specify the file full path, or a directory which meams all files under this directory include sub directories will be imported.
	
3. Click the menu "Localization/Reimport Excel Files" to import the files. The vocabulary assets will be generated in 
	> "/Assets/Wugner/Localization/Generated/Resources/"  
	
	folder, also the constant C# source file will be generated in 
	> "/Assets/Wugner/Localization/Generated/" folder.
	
4. Create a ui Text component under Canvas, add the "LocalizationText" component. Then you can set the id in your vocabulary files by selecting, and you can switch to any imported language to preview the effect.
5. By using "LocalizationImage" component, you can also set image lick text. In this case, the content in vocabulary files means the name of ths sprite than under the Resouces folder.

***

### The configuration asset
Will add soon.

### The fomart of vocabulary files
- xlsx,csv,xml are supported. It is recommended to use xml format since it is text file which can be versioned by git or other version control tools, and it can also storage many style infos by editing in Excel. You can create this xml format by using Excel's "save as" menu.
- The first row is header row, which is important. The names of head are almost fixed except the content and font should followed by language names. The column's order can be changed.
- "ID" is the identity of a vocabulary. You can use '/' to group them when output to the constant C# source file.
	A name like "**/InventoryView/Label_Title**" will result a following output.
		
	``` C#
	public class IDS
	{
		public class InventoryView
		{
			public const string Label_Title = "/InventoryView/Label_Title";
		}
	}
	```
- "**TYPE**" is used to specify the type of text or sprite. The keyword is "Text" and "Sprite".
		The type sprite will not be displayed in the selection of LocalizaitonText.
- "**CONTENT_**" is a prefix which should followed by a language name such as en or jp or what ever you like.  
	- The data in this column is the actual text to display in text mode, or a sprite name in sprite mode.  
	- You can set more than one content for multiple languages in one sheet, like the samples files in the project. Be sure these language names are the same if you seperate your data in multiple sheets or files.  
	- You won't have to add all languages in one single row. An id can appear more than once as long as the different rows have no same language column. Usually, you create vocabulary files in two or three languages which you main supported in the begining or middle phase of your development, and you would like to set those contents in one row since you can simply compare them. And in the late phase of your development, you may want to add some more language without updating those files created in early phases. This feature is made for this case.
- "**FONT_**" is also followed by the language name just like "CONTENT_".  
	- "FONT_" is a nullable parameter to specify a font which is different from the language's default font for this id. If this vocabulary in this language use the default font, set it empty.   
	- "FONT_" should appear with "CONTENT_" in pair.  
	- The language's default font can be set in LocalizationConfig.asset.
- "**REMARK**" is the comment of this id.
	- It will displayed in id selection window, and appear as a comment in the constant C# source file for every field.
	- The remark column is an optional column, since one id can appear in defferent sheets or files but the remark should be one. If multiple remarks are set for one id, one of them will be accepted but which one is depend on the order of reading files.
	- It is no use for rumtime.
- Any other column with a header besides above introduced will be ignored.

