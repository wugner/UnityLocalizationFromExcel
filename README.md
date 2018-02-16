# UnityLocalizationFromExcel
A localization tool for Unity3D based on excel documents With convenient editor selection tool and ui preview.

Below are the main features.
- Import from excel files. Such as xlsx, csv, and xml files which can be exported by Excel. (It is recommended to use xml format since it is text file which can be versioned by git or other version control tools and can alos storage many style info by editing from Excel. )
- After importing, the content from file will be storaged in unity asset files. These asset files will be loaded at runtime to display text, and also they are used in editor mode to provide id selection and preview function.
- Helpful selection and preview in ui editing based on ugui. Add LocalizationText component on a game object with Text component, you can select your imported vocabulary id with your own remark info, and can preview actual displaying for every language.
- A constant C# source file will be generated to help coding when you want to change the text at runtime in your script.
- Some images are based on languages. They are not text but should be switched when switching to another language. A LocalizationSprite component can help this situation.
- Customizable editor vocabulary importer, rumtime vocablary, manager runtime sprite manager, and runtime font manager.

Continue updating the readme...
