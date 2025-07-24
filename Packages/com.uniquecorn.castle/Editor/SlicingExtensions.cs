using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace Castle.Editor
{
    public static class SlicingExtensions
    {
        [MenuItem("CONTEXT/SpriteRenderer/Set Pivot", false, 1)]
        public static void PivotPoint(MenuCommand command)
        {
            if (command.context is SpriteRenderer sr) SetPivot(sr);
        }
        public static void SetPivot(SpriteRenderer sr)
        {
            if (sr.transform.localPosition.magnitude < 0.00001f) return;
            var assetPath = AssetDatabase.GetAssetOrScenePath(sr.sprite);
            var texImport = (TextureImporter) AssetImporter.GetAtPath(assetPath);
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(texImport);
            dataProvider.InitSpriteEditorDataProvider();
            var zPos = sr.transform.localPosition.z;
            var flip = new Vector2(sr.flipX ? -1 : 1, sr.flipY ? -1 : 1);
            var localPosToPixelPivot = sr.transform.localPosition * dataProvider.pixelsPerUnit *
                                       (Vector2) sr.transform.localScale * flip;
            var allSprites = dataProvider.GetSpriteRects();
            for (var i = 0; i < allSprites.Length; i++)
            {
                if (allSprites[i].name != sr.sprite.name) continue;
                allSprites[i].alignment = SpriteAlignment.Custom;
                allSprites[i].pivot -= new Vector2(localPosToPixelPivot.x / allSprites[i].rect.width,
                    localPosToPixelPivot.y / allSprites[i].rect.height);
            }
            dataProvider.SetSpriteRects(allSprites);
            dataProvider.Apply();
            texImport.SaveAndReimport();
            sr.transform.localPosition = new Vector3(0, 0, zPos);
        }
        [MenuItem("Assets/Slicing/Copy Pivot",priority = 0)]
        public static void CopyPivot()
        {
            if (!CopyPivotValidation(out var textureImporter)) return;
            if (textureImporter.spriteImportMode == SpriteImportMode.Single)
            {
                Sirenix.Utilities.Editor.Clipboard.Copy(textureImporter.spritePivot);
            }
            else if(Selection.activeObject is Sprite sprite)
            {
                if (!textureImporter.GetSpriteData().TryGetSpriteRect(sprite.GetSpriteID(), out var spriteRect)) return;
                Sirenix.Utilities.Editor.Clipboard.Copy(spriteRect.pivot);
            }
        }
        [MenuItem("Assets/Slicing/Copy Pivot",true)]
        public static bool CopyPivotValidation() => CopyPivotValidation(out _);
        public static bool CopyPivotValidation(out TextureImporter textureImporter)
        {
            if (Selection.count != 1 || AssetImporter.GetAtPath(
                    AssetDatabase.GetAssetPath(Selection.activeObject)) is not TextureImporter texImport)
            {
                textureImporter = null;
                return false;
            }
            textureImporter = texImport;
            if (texImport.spriteImportMode == SpriteImportMode.Single) return true;
            return Selection.activeObject is Sprite;
        }
        [MenuItem("Assets/Slicing/Paste Pivot",priority = 1)]
        public static void PastePivot()
        {
            if (!PastePivotValidation(out var pivot, out var textureImporter)) return;
            if (textureImporter.spriteImportMode == SpriteImportMode.Single)
            {
                textureImporter.GetSpriteData().ModifyAllRects(spriteRect =>
                {
                    spriteRect.alignment = SpriteAlignment.Custom;
                    spriteRect.pivot = pivot;
                    return spriteRect;
                });
                textureImporter.SaveAndReimport();
            }
            else if(Selection.activeObject is Sprite sprite)
            {
                var dataProvider = textureImporter.GetSpriteData();
                dataProvider.ModifyRect(sprite.GetSpriteID(), spriteRect =>
                {
                    spriteRect.alignment = SpriteAlignment.Custom;
                    spriteRect.pivot = pivot;
                    return spriteRect;
                });
                dataProvider.Apply();
                textureImporter.SaveAndReimport();
            }
        }
        [MenuItem("Assets/Slicing/Paste Pivot",true)]
        public static bool PastePivotValidation() => PastePivotValidation(out _,out _);
        public static bool PastePivotValidation(out Vector2 pivot,out TextureImporter textureImporter)
        {
            if (!Sirenix.Utilities.Editor.Clipboard.TryPaste(out pivot) || Selection.count != 1 || AssetImporter.GetAtPath(
                    AssetDatabase.GetAssetPath(Selection.activeObject)) is not TextureImporter texImport)
            {
                textureImporter = null;
                return false;
            }
            textureImporter = texImport;
            return Selection.activeObject switch
            {
                Texture => texImport.spriteImportMode == SpriteImportMode.Single,
                Sprite => true,
                _ => false
            };
        }
        [MenuItem("Assets/Slicing/Copy Slice",priority = 25)]
        public static void CopySlice()
        {
            if (Selection.count != 1 || Selection.activeObject is not Sprite sprite) return;
            CopySliceIntoClipboard(sprite);
        }
        [MenuItem("Assets/Slicing/Copy Slice",true)]
        public static bool CopySliceValidation() => Selection.count == 1 && Selection.activeObject is Sprite;
        [MenuItem("Assets/Slicing/Paste Slice",priority = 26)]
        public static void PasteSlice()
        {
            if (!Sirenix.Utilities.Editor.Clipboard.TryPaste(out SpriteData data)) return;
            if (Selection.count == 1 && Selection.activeObject is Sprite sprite)
            {
                PasteSliceFromClipboard(sprite,data);
            }
            else if (Selection.count > 1)
            {
                PasteSliceFromClipboard(Selection.objects.OfType<Sprite>().ToArray(),data);
            }
        }
        [MenuItem("Assets/Slicing/Paste Slice",true)]
        public static bool PasteSliceValidation()
        {
            if (!Sirenix.Utilities.Editor.Clipboard.CanPaste<SpriteData>()) return false;
            if (Selection.count == 1 && Selection.activeObject is Sprite) return true;
            return Selection.count > 1 && Selection.objects.All(x => x is Sprite);
        }
        [MenuItem("Assets/Slicing/Copy Sheet",priority = 50)]
        public static void CopySheet()
        {
            if (Selection.count != 1 || Selection.activeObject is not Texture texture) return;
            CopySheetFormat(texture);
        }
        [MenuItem("Assets/Slicing/Copy Sheet",true)]
        public static bool CopySheetValidation() => Selection.count == 1 && Selection.activeObject is Texture;
        [MenuItem("Assets/Slicing/Paste Sheet",priority = 51)]
        public static void PasteSheet()
        {
            if (!Sirenix.Utilities.Editor.Clipboard.TryPaste(out SpriteRect[] data)) return;
            if (Selection.count == 1 && Selection.activeObject is Texture texture)
            {
                PasteSheetFormat(texture,data);
            }
            else if (Selection.count > 1)
            {
                for (var i = 0; i < Selection.count; i++)
                {
                    if (Selection.objects[i] is not Texture s) continue;
                    PasteSheetFormat(s,data);
                }
            }
        }
        [MenuItem("Assets/Slicing/Paste Sheet",true)]
        public static bool PasteSheetValidation()
        {
            if (!Sirenix.Utilities.Editor.Clipboard.CanPaste<SpriteRect[]>()) return false;
            if (Selection.count == 1 && Selection.activeObject is Texture) return true;
            return Selection.count > 1 && Selection.objects.All(x => x is Texture);
        }
        [MenuItem("Assets/Slicing/Flip Sheet",priority = 75)]
        public static void FlipSheet()
        {
            if(!FlipSheetValidation(out var textureImporters)) return;
            foreach (var textureImporter in textureImporters)
            {
                FlipSprites(textureImporter);
            }
        }
        [MenuItem("Assets/Slicing/Flip Sheet", true)]
        public static bool FlipSheetValidation() => FlipSheetValidation(out _);
        public static bool FlipSheetValidation(out TextureImporter[] textureImporters)
        {
            var selectedTextures = Selection.GetFiltered<Texture>(SelectionMode.Assets);
            textureImporters = new TextureImporter[selectedTextures.Length];
            if (!selectedTextures.IsSafe()) return false;
            for (var i = 0; i < selectedTextures.Length; i++)
            {
                if (AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(selectedTextures[i])) is not TextureImporter
                    texImport)
                    return false;
                textureImporters[i] = texImport;
            }
            return true;
        }
        [MenuItem("Assets/Slicing/Flip Sprite",priority = 76)]
        public static void FlipSingleSprite()
        {
            if(!FlipSingleSpriteValidation(out var textureImporter, out var spriteGuid)) return;
            FlipSingleSprite(textureImporter,spriteGuid);
        }
        [MenuItem("Assets/Slicing/Flip Sprite", true)]
        public static bool FlipSingleSpriteValidation() => Selection.count == 1 &&
                                                           Selection.activeObject is Sprite sprite &&
                                                           sprite.texture.name != sprite.name;
        public static bool FlipSingleSpriteValidation(out TextureImporter textureImporter, out GUID spriteGuid)
        {
            if (Selection.count != 1 || Selection.activeObject is not Sprite sprite || AssetImporter.GetAtPath(
                    AssetDatabase.GetAssetPath(sprite.texture)) is not TextureImporter texImport)
            {
                textureImporter = null;
                spriteGuid = default;
                return false;
            }
            textureImporter = texImport;
            spriteGuid = sprite.GetSpriteID();
            return true;
        }
        public static void CopySliceIntoClipboard(this Sprite sprite)
        {
            var assetPath = AssetDatabase.GetAssetPath(sprite.texture);
            var texImport = (TextureImporter) AssetImporter.GetAtPath(assetPath);
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(texImport);
            dataProvider.InitSpriteEditorDataProvider();
            var outlineProvider = dataProvider.GetDataProvider<ISpriteOutlineDataProvider>();
            var allSprites = dataProvider.GetSpriteRects();
            for (var i = 0; i < allSprites.Length; i++)
            {
                if (!allSprites[i].name.Equals(sprite.name)) continue;
                Sirenix.Utilities.Editor.Clipboard.Copy(new SpriteData
                {
                    spriteRect = allSprites[i],
                    outline = outlineProvider.GetOutlines(allSprites[i].spriteID)
                });
            }
        }
        public static void PasteSliceFromClipboard(this Sprite sprite, SpriteData data)
        {
            var localOutline = new List<Vector2[]>(data.outline.Count);
            for (var i = 0; i < data.outline.Count; i++)
            {
                localOutline.Add(new Vector2[data.outline[i].Length]);
                for (var j = 0; j < localOutline[i].Length; j++)
                {
                    localOutline[i][j] = new Vector2((data.outline[i][j].x / data.spriteRect.rect.width) * sprite.rect.width,
                        (data.outline[i][j].y / data.spriteRect.rect.height) * sprite.rect.height);
                }
            }
            var assetPath = AssetDatabase.GetAssetPath(sprite.texture);
            var texImport = (TextureImporter) AssetImporter.GetAtPath(assetPath);
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(texImport);
            dataProvider.InitSpriteEditorDataProvider();
            var outlineProvider = dataProvider.GetDataProvider<ISpriteOutlineDataProvider>();
            var allSprites = dataProvider.GetSpriteRects();
            for (var i = 0; i < allSprites.Length; i++)
            {
                if (!allSprites[i].name.Equals(sprite.name)) continue;
                allSprites[i].alignment = data.spriteRect.alignment;
                allSprites[i].pivot = data.spriteRect.pivot;
                outlineProvider.SetOutlines(allSprites[i].spriteID,localOutline);
            }
            dataProvider.SetSpriteRects(allSprites);
            dataProvider.Apply();
            texImport.SaveAndReimport();
        }
        public static void PasteSliceFromClipboard(Sprite[] sprites, SpriteData data)
        {
            var assetPath = AssetDatabase.GetAssetPath(sprites[0].texture);
            var texImport = (TextureImporter) AssetImporter.GetAtPath(assetPath);
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(texImport);
            dataProvider.InitSpriteEditorDataProvider();
            var outlineProvider = dataProvider.GetDataProvider<ISpriteOutlineDataProvider>();
            var allSprites = dataProvider.GetSpriteRects();

            for (var i = 0; i < allSprites.Length; i++)
            {
                if (!sprites.Any(x => x.name.Equals(allSprites[i].name))) continue;
                allSprites[i].alignment = data.spriteRect.alignment;
                allSprites[i].pivot = data.spriteRect.pivot;

                var localOutline = new List<Vector2[]>(data.outline.Count);
                for (var k = 0; k < data.outline.Count; k++)
                {
                    localOutline.Add(new Vector2[data.outline[k].Length]);
                    for (var j = 0; j < localOutline[k].Length; j++)
                    {
                        localOutline[k][j] = new Vector2((data.outline[k][j].x / data.spriteRect.rect.width) * allSprites[i].rect.width,
                            (data.outline[k][j].y / data.spriteRect.rect.height) * allSprites[i].rect.height);
                    }
                }

                outlineProvider.SetOutlines(allSprites[i].spriteID,localOutline);
            }
            dataProvider.SetSpriteRects(allSprites);
            dataProvider.Apply();
            texImport.SaveAndReimport();
        }
        public static void CopySheetFormat(Texture texture)
        {
            var assetPath = AssetDatabase.GetAssetPath(texture);
            var texImport = (TextureImporter) AssetImporter.GetAtPath(assetPath);
            if(texImport.spriteImportMode != SpriteImportMode.Multiple) return;
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(texImport);
            dataProvider.InitSpriteEditorDataProvider();
            var rects = dataProvider.GetSpriteRects();
            for (var i = 0; i < rects.Length; i++)
            {
                rects[i].spriteID = new GUID();
                rects[i].name = rects[i].name.Replace(texture.name, "textureName");
            }
            Sirenix.Utilities.Editor.Clipboard.Copy(rects);
        }
        public static void PasteSheetFormat(Texture texture,SpriteRect[] data)
        {
            var assetPath = AssetDatabase.GetAssetPath(texture);
            var texImport = (TextureImporter) AssetImporter.GetAtPath(assetPath);
            texImport.spriteImportMode = SpriteImportMode.Multiple;
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(texImport);
            dataProvider.InitSpriteEditorDataProvider();
            for (var i = 0; i < data.Length; i++)
            {
                data[i].name = data[i].name.Replace("textureName", texture.name);
            }
            dataProvider.SetSpriteRects(data);
            dataProvider.Apply();
            texImport.SaveAndReimport();
        }
        public static void FlipSprites(TextureImporter textureImporter)
        {
            var dataProvider = GetSpriteData(textureImporter,out var outlineProvider);
            var allSprites = dataProvider.GetSpriteRects();
            textureImporter.GetSourceTextureWidthAndHeight(out var textureWidth,out _);
            for (var i = 0; i < allSprites.Length; i++)
            {
                allSprites[i].rect =
                    new Rect(textureWidth - (allSprites[i].rect.x + allSprites[i].rect.width), allSprites[i].rect.y,
                        allSprites[i].rect.width, allSprites[i].rect.height);
                allSprites[i].pivot = new Vector2(Mathf.Lerp(1, 0, allSprites[i].pivot.x), allSprites[i].pivot.y);
                outlineProvider.ModifyOutline(allSprites[i].spriteID, outlines =>
                {
                    for (var j = 0; j < outlines.Count; j++)
                    {
                        for (var k = 0; k < outlines[j].Length; k++)
                        {
                            outlines[j][k] = new Vector2(-outlines[j][k].x, outlines[j][k].y);
                        }
                    }
                });
            }
            dataProvider.SetSpriteRects(allSprites);
            dataProvider.Apply();
            textureImporter.SaveAndReimport();
        }
        public static void FlipSingleSprite(TextureImporter textureImporter, GUID spriteGuid)
        {
            var dataProvider = GetSpriteData(textureImporter,out var outlineProvider);
            dataProvider.ModifyRect(spriteGuid, spriteRect =>
            {
                spriteRect.pivot = new Vector2(Mathf.Lerp(1, 0, spriteRect.pivot.x), spriteRect.pivot.y);
                return spriteRect;
            });
            outlineProvider.ModifyOutline(spriteGuid, (outlines) =>
            {
                for (var j = 0; j < outlines.Count; j++)
                {
                    for (var k = 0; k < outlines[j].Length; k++)
                    {
                        outlines[j][k] = new Vector2(-outlines[j][k].x, outlines[j][k].y);
                    }
                }
            });
            dataProvider.Apply();
            textureImporter.SaveAndReimport();
        }
        public static ISpriteEditorDataProvider GetSpriteData(this TextureImporter textureImporter)
        {
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(textureImporter);
            dataProvider.InitSpriteEditorDataProvider();
            return dataProvider;
        }
        public static ISpriteEditorDataProvider GetSpriteData(this TextureImporter textureImporter, out ISpriteOutlineDataProvider outlineProvider)
        {
            var dataProvider = GetSpriteData(textureImporter);
            outlineProvider = dataProvider.GetDataProvider<ISpriteOutlineDataProvider>();
            return dataProvider;
        }
        public static void ModifyAllRects(this ISpriteEditorDataProvider dataProvider, System.Func<SpriteRect,SpriteRect> modification)
        {
            var allSprites = dataProvider.GetSpriteRects();
            for (var i = 0; i < allSprites.Length; i++)
            {
                allSprites[i] = modification(allSprites[i]);
            }
            dataProvider.SetSpriteRects(allSprites);
        }
        public static void ModifyRect(this ISpriteEditorDataProvider dataProvider, GUID spriteGuid, System.Func<SpriteRect,SpriteRect> modification)
        {
            var allSprites = dataProvider.GetSpriteRects();
            for (var i = 0; i < allSprites.Length; i++)
            {
                if (allSprites[i].spriteID != spriteGuid) continue;
                allSprites[i] = modification(allSprites[i]);
                break;
            }
            dataProvider.SetSpriteRects(allSprites);
        }
        public static bool TryGetSpriteRect(this ISpriteEditorDataProvider dataProvider, GUID spriteGuid, out SpriteRect spriteRect)
        {
            var allSprites = dataProvider.GetSpriteRects();
            for (var i = 0; i < allSprites.Length; i++)
            {
                if (allSprites[i].spriteID != spriteGuid) continue;
                spriteRect = allSprites[i];
                return true;
            }

            spriteRect = null;
            return false;
        }
        public static void ModifyOutline(this ISpriteOutlineDataProvider outlineProvider, GUID spriteGuid,
            System.Action<List<Vector2[]>> modification)
        {
            var outlines = outlineProvider.GetOutlines(spriteGuid);
            modification(outlines);
            outlineProvider.SetOutlines(spriteGuid,outlines);
        }
        [System.Serializable]
        public struct SpriteData
        {
            public SpriteRect spriteRect;
            public List<Vector2[]> outline;
        }
    }
}