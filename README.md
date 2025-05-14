# Ventixe Frontend

## Icons

The icons used in this project are from Material Symbols.

Search an icon here: https://fonts.google.com/icons?icon.style=Rounded 

In _Layout.cshtml we import the icon stylesheet. To optimize loading times we select which icons are fetched, as recommended here: https://developers.google.com/fonts/docs/material_symbols#optimize_the_icon_font

We select the icon by adding the icon name in the **icon_names=** parameter.

The icon names must be in alphabetical order.

In this example the check_box, grid_view, and logout icons are selected:
```
 <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Symbols+Rounded:opsz,wght,FILL,GRAD@24,300,0,0&icon_names=check_box,grid_view,logout&display=block" />
```

Then use it like this: 
```
<span class="material-symbols-rounded">
	grid_view
</span>
```
