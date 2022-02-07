## Setup
Add this line to your Packages/manifest.json dependencies array;
```json
    "com.kingenderbrine.rorskinbuilder": "https://github.com/KingEnderBrine/RoRSkinBuilder.git",
```

## Usage
First, create Prefab Child Reference thourh Assets/Create menu
![](https://cdn.discordapp.com/attachments/706089456855154778/940260753032773744/unknown.png)

Then open prefab that has `GameObject` that you want, select your Prefab Child Reference asset, add new element to `References` array, then just drag and drop `GameObject` to `Referenced Object` field and hit `Apply`.
![](https://cdn.discordapp.com/attachments/706089456855154778/940261733375815710/unknown.png)

That's it, now you can use your Prefab Child Reference as you would use any prefab. Any changes to the original prefab would automatically be applied to Prefab Child Reference
![](https://cdn.discordapp.com/attachments/706089456855154778/940262099102363678/unknown.png)