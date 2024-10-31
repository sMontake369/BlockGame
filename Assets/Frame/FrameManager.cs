using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FrameManager : MonoBehaviour
{
    MainGameManager GamM;
    BattleManager BatM;
    BorderInt frameBorder;
    
    List<List<FrameBox>> frameListList = new List<List<FrameBox>>();
    //public List<List<FrameBox>> FrameListList { get => frameListList; private set => FrameListList = value; }

    public BorderInt LFrameBorder { get => frameBorder; }
    public BorderInt WFrameBorder { get => BatM.battlePos + frameBorder; }

    public void Init(BattleManager BatM)
    {
        this.BatM = BatM;
        GamM = BatM.GamM;
        frameBorder = new BorderInt(new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 0));

        if(!BatM || !GamM) 
        {
            Debug.Log("BattleManager or MainGameManager is not found");
            return;
        }
    }

    List<List<FrameBox>> GenerateFrame(FrameData frameData) //フレームを生成
    {
        List<List<FrameBox>> newFrameListList = new List<List<FrameBox>>();
        for(int y = 0; y < frameData.frameSize.y; y++)
        {
            newFrameListList.Add(new List<FrameBox>());
            for(int x = 0; x < frameData.frameSize.x; x++)
            {
                newFrameListList[y].Add(new FrameBox());
                if(frameData.indexDataList[(y * frameData.frameSize.x) + x] == -2) newFrameListList[y][x].canMove = false;
                else newFrameListList[y][x].canMove = true;

                if(frameData.indexDataList[(y * frameData.frameSize.x) + x] <= -1) continue;
                
                if(frameData.blockDataList.Count <= frameData.indexDataList[(y * frameData.frameSize.x) + x])
                {
                    Debug.Log("FrameManager: 存在しないブロックデータが指定されています");
                    continue;
                }
                BlockData blockData = frameData.blockDataList[frameData.indexDataList[(y * frameData.frameSize.x) + x]];
                BaseBlock baseBlock = GamM.GenerateBlock(blockData.blockType, blockData.colorType, blockData.texture);
                baseBlock.transform.parent = this.transform;
                newFrameListList[y][x].SetBlock(baseBlock);
            }
        }
        return newFrameListList;
    }

    public Vector3Int SetFrame(FrameData frameData) //いつかframeのクラスを作成し、それを返すようにしたい
    {
        DeleteAllFrame();

        frameListList = GenerateFrame(frameData);
        for(int y = 0; y < frameListList.Count; y++)
        for(int x = 0; x < frameListList[y].Count; x++)
        {
            if(!frameListList[y][x].IsContain()) continue;
            frameListList[y][x].BaseBlock.transform.localPosition = this.transform.localPosition + new Vector3(x, y, 0);
        }
        Vector3Int frameSize = new Vector3Int(frameListList[0].Count, frameListList.Count, 0);
        frameBorder.SetMinMax(Vector3Int.zero, frameSize - new Vector3Int(1, 1, 0)); //配列は0から始まるので-1

        return frameSize;
    }

    public void AddFrame(Vector3Int pos, FrameData frameData, bool axisX)
    {
        if(!IsWithinBoard(pos))
        {
            Debug.Log("FrameManager: AddFrame: pos is out of board");
            return;
        }

        if(GamM.playerBlock) DeleteRBlock(GamM.playerBlock);
        List<List<FrameBox>> newFrameListList = GenerateFrame(frameData);
        Vector3Int frameSize = new Vector3Int(newFrameListList[0].Count, newFrameListList.Count, 0);

        Vector3Int insertNum = CheckFrameBox(pos, frameSize, axisX);
        FrameUtility.InsertListList(frameListList, pos, insertNum);

        for(int y = pos.y; y < frameListList.Count; y++)
        for(int x = pos.x; x < frameListList[y].Count; x++)
        {
            if(frameListList[y][x] == null) continue;
            if(!frameListList[y][x].IsContain()) continue;
            frameListList[y][x].BaseBlock.frameIndex += insertNum;
            frameListList[y][x].BaseBlock.transform.position += insertNum;
        }

        for(int y = 0; y < newFrameListList.Count; y++)
        for(int x = 0; x < newFrameListList[y].Count; x++)
        {
            frameListList[pos.y + y][pos.x + x] = newFrameListList[y][x];

            if(!newFrameListList[y][x].IsContain()) continue;
            newFrameListList[y][x].BaseBlock.transform.position = this.transform.position + pos + new Vector3(x, y, 0);
        }

        frameBorder.max += insertNum;
        if(GamM.playerBlock) 
        {
            SetRBlock(GamM.playerBlock);
            GamM.playerBlock.GenerateGhostBlock();
        }
    }

    public void DeleteAllFrame()
    {
        DeleteAllBlocks();
        frameListList.Clear();
        frameBorder.max = Vector3Int.zero;
    }

    Vector3Int CheckFrameBox(Vector3Int pos, Vector3Int size, bool axisX)
    {
        Vector3Int vector3Int = new Vector3Int(0, 0, 0);
        if(axisX) goto CheckX;
        
        for(int y = pos.y; y < pos.y + size.y; y++)
        {
            if(frameListList.Count <= y) 
            {
                vector3Int.y++;
                continue;
            }
            foreach(FrameBox frameBox in frameListList[y]) 
            {
                if(frameBox.canMove) 
                {
                    vector3Int.y++;
                    break;
                }
            }
        }
        return vector3Int;
        CheckX:

        for(int x = pos.x; x < pos.x + size.x; x++)
        {
            foreach(List<FrameBox> frameBoxList in frameListList) 
            {
                if(frameBoxList.Count <= x) 
                {
                    vector3Int.x++;
                    continue;
                }
                if(frameBoxList[x].canMove) 
                {
                    vector3Int.x++;
                    break;
                }
            }
        }
        return vector3Int;
    }

    public void DeleteFrame(Vector3Int pos, Vector3Int size)
    {
        if(!IsWithinBoard(pos) || !IsWithinBoard(pos + size))
        {
            Debug.Log("FrameManager: DeleteFrame: pos is out of board");
            return;
        }

        for(int y = pos.y; y < size.y; y++)
        for(int x = pos.x; x < size.x; x++)
        {
            frameListList[y][x].Delete();
            frameListList[y][x].canMove = false;
        }
    }

    public bool IsWithinBoard(Vector3Int pos) //posがボード内かどうか
    {
        if(LFrameBorder.lowerLeft.x <= pos.x && pos.x <= LFrameBorder.upperRight.x && 
        LFrameBorder.lowerLeft.y <= pos.y && pos.y <= LFrameBorder.upperRight.y) return true;
        else return false;
    }

    public bool IsConflict(RootBlock rootBlock, Vector3Int offset) //ブロックが衝突しているかどうか
    {
        foreach(BaseBlock baseBlock in rootBlock.BlockList)
        if(IsConflict(baseBlock, offset)) return true;
        
        return false;
    }

    public bool IsConflict(BaseBlock block, Vector3Int offset) //ブロックが衝突しているかどうか
    {
        if(block == null) return false;
        Vector3Int pos = block.frameIndex + offset;

        //ボード外に出ているかどうか
        if(!IsWithinBoard(pos)) return true;

        //ブロックが存在しているかどうか
        if(!frameListList[pos.y][pos.x].canMove) return false;
        if(!frameListList[pos.y][pos.x].IsContain()) return false;
        if(frameListList[pos.y][pos.x].BaseBlock.RootBlock == block.RootBlock || 
        frameListList[pos.y][pos.x].BaseBlock.blockType == BlockType.Ghost ||
        frameListList[pos.y][pos.x].BaseBlock.RootBlock == GamM.playerBlock) return false; //ブロックが自分自身か、ゴーストブロックかどうか
        else return true;
    }

    public RootBlock DeleteRBlock(RootBlock rootBlock) //ルートブロックをボードから消す
    {
        if(rootBlock == null) return null;
        for(int y = 0; y < rootBlock.BlockListList.Count; y++)
        for(int x = 0; x < rootBlock.BlockListList[y].Count; x++)
        DeleteBlock(rootBlock.BlockListList[y][x]);

        if(GamM.playerBlock) GamM.playerBlock.GenerateGhostBlock();
        return rootBlock;
    }

    public BaseBlock DeleteBlock(BaseBlock block) //ベースブロックをフレームから消す よそのブロックは消せない
    {
        if(block == null) return null;
        if(!IsWithinBoard(block.frameIndex)) return null;
        if(frameListList[block.frameIndex.y][block.frameIndex.x].IsContain() && 
        frameListList[block.frameIndex.y][block.frameIndex.x].BaseBlock.RootBlock == block.RootBlock) 
        {
            return frameListList[block.frameIndex.y][block.frameIndex.x].Delete();
        }
        return null;
    }

    public List<BaseBlock> DeleteBlocks(Vector3Int from, Vector3Int to) //指定範囲のブロックをボードから消す
    {
        List<BaseBlock> blockList = new List<BaseBlock>();
        for(int y = from.y; y < to.y; y++)
        for(int x = from.x; x < to.x; x++)
        {
            if(!frameListList[y][x].IsContain()) continue;
            blockList.Add(frameListList[y][x].Delete());
        }
        return blockList;
    }

    public List<BaseBlock> DeleteAllBlocks() //全てのブロックをボードから消す
    {
        List<BaseBlock> blockList = new List<BaseBlock>();
        for(int y = 0; y < frameListList.Count; y++)
        for(int x = 0; x < frameListList[y].Count; x++)
        {
            if(!frameListList[y][x].IsContain()) continue;
            blockList.Add(frameListList[y][x].Delete());
        }
        return blockList;
    }

    public bool SetRBlock(RootBlock rootBlock) //ルートブロックをボードにセット　//ベースブロックをボードにセット Vector2Int posを追加すべき
    {
        if(rootBlock == null) return false;

        foreach(BaseBlock block in rootBlock.BlockList)
        SetBlock(block, true);

        if(GamM.playerBlock) GamM.playerBlock.GenerateGhostBlock(); //GamMがプレイヤーブロックを持っているかどうか確認してほしい
        GamM.CheckLine();
        return true;
    }

    public bool SetBlock(BaseBlock block, bool onlySet = false) //ベースブロックをボードにセット Vector2Int posを追加すべき
    {
        if(block == null) return false;
        if(!IsWithinBoard(block.frameIndex)) return false;
        if(frameListList[block.frameIndex.y][block.frameIndex.x].IsContain()) return false;

        frameListList[block.frameIndex.y][block.frameIndex.x].SetBlock(block);

        if(!onlySet && GamM.playerBlock) GamM.playerBlock.GenerateGhostBlock(); //GamMがプレイヤーブロックを持っているかどうか確認してほしい

        return true;
    }

    public BaseBlock GetBlock(Vector3Int pos) //指定位置のベースブロックを取得
    {
        if(!IsWithinBoard(pos)) return null;
        if(!frameListList[pos.y][pos.x].IsContain()) return null;
        return frameListList[pos.y][pos.x].BaseBlock;
    }

    public List<List<BaseBlock>> GetBlocks(Vector3Int from, Vector3Int to) //指定範囲のベースブロックリストを取得
    {
        if(!IsWithinBoard(from) && !IsWithinBoard(to)) return null;
        List<List<BaseBlock>> blockListList = new List<List<BaseBlock>>();
        for(int y = from.y; y < to.y; y++)
        {
            List<BaseBlock> blockList = new List<BaseBlock>();
            for(int x = from.x; x < to.x; x++)
            {
                if(!frameListList[y][x].IsContain()) continue;
                blockList.Add(frameListList[y][x].BaseBlock);
            }
            blockListList.Add(blockList);
        }
        return blockListList;
    }

    public List<BaseBlock> GetBlockLine(int y)
    {
        if(y < 0 || y >= frameBorder.max.y) return null;
        
        List<BaseBlock> blockList = new List<BaseBlock>();
        for(int x = 0; x < frameListList[y].Count; x++)
        {
            if(!frameListList[y][x].IsContain()) continue;
            blockList.Add(frameListList[y][x].BaseBlock);
        }
        return blockList;
    }

    public List<RootBlock> GetRBlockLine(int y)
    {
        if(y < 0 || y >= frameListList.Count) return null;
        List<RootBlock> rootBlockList = new List<RootBlock>();
        for(int x = 0; x < frameListList[y].Count; x++)
        {
            if(!frameListList[y][x].IsContain()) continue;
            if(frameListList[y][x].BaseBlock.RootBlock == GamM.playerBlock) continue;
            if(!rootBlockList.Contains(frameListList[y][x].BaseBlock.RootBlock)) rootBlockList.Add(frameListList[y][x].BaseBlock.RootBlock);
        }
        return rootBlockList;
    }

    public List<RootBlock> GetRBlocks(Vector3Int from, Vector3Int to) //指定範囲のルートブロックリストを取得 //ラインじゃなくて範囲にしたい なくてもいいかも
    {
        if(!IsWithinBoard(from) || !IsWithinBoard(to)) return null;
        List<RootBlock> rootBlockList = new List<RootBlock>();
        for(int y = from.y; y <= to.y; y++)
        for(int x = from.x; x <= to.x; x++)
        {
            if(!frameListList[y][x].IsContain()) continue;
            if(frameListList[y][x].BaseBlock.blockType != BlockType.Mino) continue;
            if(frameListList[y][x].BaseBlock.RootBlock == GamM.playerBlock) continue;
            if(!rootBlockList.Contains(frameListList[y][x].BaseBlock.RootBlock)) rootBlockList.Add(frameListList[y][x].BaseBlock.RootBlock);
        }
        return rootBlockList;
    }

    public void OnPlayerGround() //プレイヤーブロックが地面に着地した時
    {
        for(int y = 0; y < frameListList.Count; y++)
        for(int x = 0; x < frameListList[y].Count; x++)
        {
            if(!frameListList[y][x].IsContain()) continue;
            frameListList[y][x].OnPlayerGround();
        }
    }
}