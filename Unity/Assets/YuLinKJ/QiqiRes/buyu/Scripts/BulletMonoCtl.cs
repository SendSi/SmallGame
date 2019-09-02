using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETModel
{
    public class BulletMonoCtl : MonoBehaviour
    {
        public int playerid;
        private Vector3 m_v3PosSaved;
        private Vector3 m_v3CurPos;
        private Vector3 m_v3Euler;
        private float speed = 10f;
        public Camera uicamera;
        public int buttletType;
        public Transform targetFish;
        private void Start()
        {
            
            uicamera = GameObject.Find("Global/Camera/UICamera").GetComponent<Camera>();
            speed = float.Parse(transform.Find("sp").GetComponent<Text>().text);
        }

        public void Reset()
        {
            isCollect = false;
            this.targetFish = null;
        }

        public void SetTarget(Transform trTarget)
        {
            this.buttletType = 1;
            this.targetFish = trTarget;
        }
        void Update()
        {
            if (!isCollect)
            {
                if (this.buttletType == 1)
                {

                    if (this.targetFish != null)
                    {
                        this.Move(this.targetFish);
                    }
                    else
                    {
                        this.buttletType = 0;
                        this.Move();
                    }
                }
                else
                {
                    this.Move();
                }
                if (transform.localPosition.x > 1618f || transform.localPosition.x < -1618f || transform.localPosition.y > 1050f || transform.localPosition.y < -1050f)
                {
                    //Debug.Log("BulletMonoCtl子弹移除");
                    //FishManager.bulletlist.Remove(gameObject);
                    Collect();
                    //Destroy(this.gameObject);
                }
            }

        }
        void Move(bool autoDesroy = false)
        {

            //   Debug.Log("………………子弹飞");
            this.m_v3PosSaved = this.transform.localPosition;
            transform.Translate(Vector3.up * speed * Time.deltaTime);
            this.m_v3CurPos = this.transform.localPosition;
            Vector2 viewPos = uicamera.WorldToViewportPoint(this.transform.position);
            if ((viewPos.y > 1) || (viewPos.y < 0f))
            {
                this.transform.localPosition = this.m_v3PosSaved;
                this.m_v3Euler = this.transform.localEulerAngles;
                this.m_v3Euler.z = 180f - this.m_v3Euler.z;
                this.transform.localEulerAngles = this.m_v3Euler;
                if (autoDesroy)
                {
                    Collect();
                    //Destroy(gameObject);
                    return;
                }
            }
            if ((viewPos.x > 1) || (viewPos.x < 0f))
            {
                this.transform.localPosition = this.m_v3PosSaved;
                this.m_v3Euler = this.transform.localEulerAngles;
                this.m_v3Euler.z *= -1f;
                this.transform.localEulerAngles = this.m_v3Euler;
                if (autoDesroy)
                {
                    Collect();
                    //Destroy(gameObject);
                }
            }
            

        }
        void Move(Transform trTarget)
        {

            //  Debug.Log("……………………子弹朝向飞");
            this.m_v3PosSaved = this.transform.localPosition;
            Vector3 up = this.transform.up;
            Vector3 vector2 = (Vector3)((this.speed * Time.deltaTime) * up);
            Vector2 viewPos = uicamera.WorldToViewportPoint(this.transform.position);
            float magnitude = vector2.magnitude;
            float num2 = Vector3.Distance(this.transform.position, trTarget.position);
            if (magnitude > (num2 * 10f))
            {
                this.transform.position = trTarget.position;
                //   Destroy(gameObject);
            }
            else
            {
                transform.Translate(Vector3.up * speed * Time.deltaTime);
                // this.transform.localPosition += vector2;
                Vector3 to = trTarget.position - this.transform.position;
                float zAngle = Vector3.Angle(up, to);
                if (zAngle > 20f)
                {
                    Collect();
                    //Destroy(gameObject);
                }
                else
                {
                    float num4 = (Vector3.Cross(up, to).z <= 0f) ? ((float)(-1)) : ((float)1);
                    zAngle *= num4;
                    if ((viewPos.y > 1) || (viewPos.y < 0f))
                    {
                        Collect();
                        //Destroy(gameObject);
                    }
                    else if ((viewPos.x > 1) || (viewPos.x < 0f))
                    {
                        Collect();
                        //Destroy(gameObject);
                    }
                    else
                    {
                        this.transform.Rotate(0f, 0f, zAngle);
                        this.m_v3CurPos = this.transform.localPosition;
                    }
                }
            }
            
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            //Debug.Log("bullet OnTriggerEnter2D");
            //if (collision.GetComponent<TagEnum>().QPTag== TagEnum.QPTags.QPBorder)
            //{
            //    //FishManager.bulletlist.Remove(gameObject);
            //    //  Debug.Log("BulletMonoCtl子弹旋转角度X:" + transform.localEulerAngles.x + "Y:" + transform.localEulerAngles.y + "Z" + transform.localEulerAngles.z);
            //    Collect();
            //    //Destroy(gameObject);

            //}
            //if (collision.GetComponent<TagEnum>().QPTag == TagEnum.QPTags.QPFish)
            //{
            //    if (targetFish == null)
            //    {
            //        //FishManager.bulletlist.Remove(gameObject);
            //        Collect();
            //        //Destroy(gameObject);
            //    }
            //    else
            //    {
                    
            //        if (FishManager.IsLock && FishManager.AnimFish != null)
            //        {
            //            if (collision.gameObject == FishManager.AnimFish)
            //            {
            //                //FishManager.bulletlist.Remove(gameObject);
            //                Collect();
            //                //Destroy(gameObject);
            //            }
            //        }
            //        else
            //        {
            //            //FishManager.bulletlist.Remove(gameObject);
            //            Collect();
            //            //Destroy(gameObject);
            //        }
            //    }
            //}
        }

        public bool isCollect = false;
        private System.Action<GameObject> collectAction;
        public void SetCollectAction(System.Action<GameObject> _collectAction) {

            collectAction = _collectAction;
        }

        public void Collect() {

            if (!isCollect) {

                isCollect = true;
                if (collectAction != null) collectAction(this.gameObject);
                else Destroy(gameObject);
            }
        }
    }
}
