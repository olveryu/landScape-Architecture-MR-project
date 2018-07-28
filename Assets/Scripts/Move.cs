using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
public class Move : NetworkBehaviour {
    public struct TransSimple {
        public Vector3 position;
        public Vector3 euler;
    }
    public class TransSyncList : SyncListStruct<TransSimple> { }
    public bool sync = true;
    public TransSyncList transSyncList = new TransSyncList();//同步结构体列表
    public List<Transform> proxyList;//需要同步的网络代理体列表
    [Header("同步频率（帧）")]
    public int syncRateFrames = 10;
    [Header("插值步长")]
    public float LerpStep = 1.6f;
    private TransSimple currSync;
    private Transform currProxy;
    private TransSimple[] oldTrans;//远程数据更新时，代理体当前位置和旋转值
    private TransSimple oldTransItem;
    private TransSimple transTemp;
    private float[] pXArr;
    private float[] pYArr;
    private float[] pZArr;
    private float[] eXArr;
    private float[] eYArr;
    private float[] eZArr;
    private float[] lerpTimes;
    private int SyncFlag = 0;
    private float t = 0f;

    private void Start() {
        transSyncList.Callback = TransChanged;
        transTemp = new TransSimple();
        InitArr();
    }

    void FixedUpdate() {
        if (!sync) return;
        if (hasAuthority) {
            SyncFlag++;
            if (SyncFlag >= syncRateFrames) {
                SyncFlag = 0;
                if (proxyList != null && proxyList.Count > 0) {
                    for (int i = 0; i < proxyList.Count; i++) {
                        pXArr[i] = proxyList[i].position.x;
                        pYArr[i] = proxyList[i].position.y;
                        pZArr[i] = proxyList[i].position.z;
                        eXArr[i] = proxyList[i].eulerAngles.x;
                        eYArr[i] = proxyList[i].eulerAngles.y;
                        eZArr[i] = proxyList[i].eulerAngles.z;
                    }
                    CmdChangTran(pXArr, pYArr, pZArr, eXArr, eYArr, eZArr);
                }
            }
        }
        else {
            LerpTransfrom();
        }
    }
    /// <summary>
    /// 服务器端更新同步结构体列表值
    /// </summary>
    /// <param name="_pxArr"></param>
    /// <param name="_pyArr"></param>
    /// <param name="_pzArr"></param>
    /// <param name="_exArr"></param>
    /// <param name="_eyArr"></param>
    /// <param name="_ezArr"></param>
    [Command]
    public void CmdChangTran(float[] _pxArr, float[] _pyArr, float[] _pzArr, float[] _exArr, float[] _eyArr, float[] _ezArr) {
        if (transSyncList != null) {
            for (int _itemIndex = 0; _itemIndex < _pxArr.Length; _itemIndex++) {
                if (transSyncList.Count > _itemIndex) {
                    transTemp = transSyncList[_itemIndex];
                    SetTrans(_pxArr[_itemIndex], _pyArr[_itemIndex], _pzArr[_itemIndex], _exArr[_itemIndex], _eyArr[_itemIndex], _ezArr[_itemIndex], out transTemp);
                    transSyncList[_itemIndex] = transTemp;
                    transSyncList.Dirty(_itemIndex);
                }
                else if (transSyncList.Count == _itemIndex) {
                    SetTrans(_pxArr[_itemIndex], _pyArr[_itemIndex], _pzArr[_itemIndex], _exArr[_itemIndex], _eyArr[_itemIndex], _ezArr[_itemIndex], out transTemp);
                    transSyncList.Add(transTemp);
                }
                else {
                    for (int i = 0; i < _itemIndex - transSyncList.Count; i++) {
                        transSyncList.Add(transTemp);
                    }
                    SetTrans(_pxArr[_itemIndex], _pyArr[_itemIndex], _pzArr[_itemIndex], _exArr[_itemIndex], _eyArr[_itemIndex], _ezArr[_itemIndex], out transTemp);
                    transSyncList.Add(transTemp);
                }
            }
        }
    }
    private void SetTrans(float _px, float _py, float _pz, float _ex, float _ey, float _ez, out TransSimple _transTemp) {
        _transTemp.position.x = _px;
        _transTemp.position.y = _py;
        _transTemp.position.z = _pz;
        _transTemp.euler.x = _ex;
        _transTemp.euler.y = _ey;
        _transTemp.euler.z = _ez;
    }
    /// <summary>
    /// 初始化数组
    /// </summary>
    private void InitArr() {
        int syncCount = proxyList.Count;
        pXArr = new float[syncCount];
        pYArr = new float[syncCount];
        pZArr = new float[syncCount];
        eXArr = new float[syncCount];
        eYArr = new float[syncCount];
        eZArr = new float[syncCount];
        lerpTimes = new float[syncCount];
        for (int i = 0; i < lerpTimes.Length; i++) {
            lerpTimes[i] = syncRateFrames;
        }
        oldTrans = new TransSimple[syncCount];
        for (int i = 0; i < oldTrans.Length; i++) {
            ResetOldTransform(i);
        }


    }
    /// <summary>
    /// 设置旧的【代理体当前的】位置和旋转，用于插值起始值
    /// </summary>
    /// <param name="index"></param>
    private void ResetOldTransform(int index) {
        transTemp.position = proxyList[index].position;
        transTemp.euler = proxyList[index].eulerAngles;
        oldTrans[index] = transTemp;


    }
    /// <summary>
    /// 同步结构体列表数据变化回调
    /// </summary>
    /// <param name="op"></param>
    /// <param name="itemIndex"></param>
    void TransChanged(SyncListStruct<TransSimple>.Operation op, int itemIndex) {
        if (!sync) return;
        if (hasAuthority) return;
        if (proxyList != null && proxyList.Count > itemIndex &&
                (op.Equals(SyncListStruct<TransSimple>.Operation.OP_ADD) ||
                op.Equals(SyncListStruct<TransSimple>.Operation.OP_DIRTY)
                )
            ) {
            lerpTimes[itemIndex] = syncRateFrames;
            ResetOldTransform(itemIndex);
        }
    }
    /// <summary>
    /// 插值平滑位置和旋转
    /// </summary>
    private void LerpTransfrom() {
        for (int i = 0; i < proxyList.Count; i++) {
            lerpTimes[i] -= LerpStep;
            if (lerpTimes[i] <= 0) {
                continue;
            }
            else {
                if (transSyncList.Count > i) {
                    currSync = transSyncList[i];
                    currProxy = proxyList[i];
                    oldTransItem = oldTrans[i];
                    t = (float)(syncRateFrames - lerpTimes[i]) / (float)syncRateFrames;
                    currProxy.position = Vector3.Lerp(oldTransItem.position, currSync.position, t);
                    currProxy.rotation = Quaternion.Lerp(Quaternion.Euler(oldTransItem.euler), Quaternion.Euler(currSync.euler), t);
                }
            }
        }
    }
}