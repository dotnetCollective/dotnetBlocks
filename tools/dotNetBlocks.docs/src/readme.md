<!---
Title: DotNetBlocks.Docs
NavigationTitle: DotNetBlocks.Docs
ShowInNavigation: false
ShowInSidebar: true
NoSidebar: false
Excerpt: Describes document generation details.

--->
# documentation

This class generates the documentation from DotNetBlocks using fxDoc library.

We use the DocFx library to generate the documentation located in and under the Docs folder.

Content of the api folder is output from DocFx reading the assemblies and classes and generating documentation.

For written content we organize markdown files  under the docs folder.
Each md file contains a header yaml block inside html comments. This ensures the header does not appear during rendering.

## Yaml headers:

### Functional
* Title: Title of the document
* NavigationTitle: used in Nav elements
* Excerpt: Short summary of the document contents.
### Future
 * Level: Level in table of contents.
* Order: sort Order in table of contents
* Show In Navigation: boolean True to show in navigation menu.
* NoSideBar: Hide sidebar for this page.
* BreadcrumbTitle: Title to show in breadcumb.

 # toc.yml

 DocFx requires a toc.yml file in every folder describing the files in that folder or below with rendering details extracted from the yaml front matter.

Extracting the Front Matter.

Markdig extracts the html comment block and is used in a second pass to extract the yaml content.

YamlDotNet converts the yaml content into an object representing the front matter.
Toc files are generated in each folder using Markdig and the information extracted from the front matter.

