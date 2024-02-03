using UnityEngine;
using QFramework;
using UnityEngine.UI;

namespace StarScavenger
{
    public enum PlanetType
    {
        Ice,
        Ocean,
        Ring,
        Forest,
        Earth,
        Tech,
        Sun,
    }

    public partial class Planet : ViewController
    {
        public bool IsDiscover = false;
        public bool IsArrived = false;
        public PlanetType Type;
        public bool IsRotate = true;
        public float MinRotationSpeed = 0.1f;
        public float MaxRotationSpeed = 1f;
        // 星球表面的重力加速度
        public float Gravity = 10f;
        public float Radius = 1.5f;
        private float mRandomRotationSpeed;

        private void Start()
        {
            Radius = HitHurtBox.radius;

            mRandomRotationSpeed = Random.Range(MinRotationSpeed, MaxRotationSpeed);

            // 重力影响区域
            GravityArea.OnTriggerEnter2DEvent(collider2D =>
            {
                HitHurtBox hitHurtBox = collider2D.GetComponent<HitHurtBox>();
                if (hitHurtBox != null)
                {
                    Player player = Player.Default;
                    if (hitHurtBox.Owner.CompareTag("Player"))
                    {
                        Text title = GamePanel.Default.SceneTitleText;
                        Text description = GamePanel.Default.SmallTitleText;

                        switch (Type)
                        {
                            case PlanetType.Ice:
                                title.text = "冰星";
                                break;
                            case PlanetType.Ocean:
                                title.text = "海星";
                                break;
                            case PlanetType.Ring:
                                title.text = "环星";
                                break;
                            case PlanetType.Forest:
                                title.text = "绿星";
                                break;
                            case PlanetType.Earth:
                                title.text = "蓝星";
                                break;
                            case PlanetType.Tech:
                                title.text = "红星";
                                break;
                            case PlanetType.Sun:
                                title.text = "恒星";
                                break;
                            default:
                                break;
                        }

                        description.text = "接近中";

                        title.Show()
                            .Delay(3f, () =>
                            {
                                title.Hide();
                            }).Execute();

                        description.Show().Delay(3f, () =>
                            {
                                description.Hide();
                            }).Execute();
                    }
                }

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            GravityArea.OnTriggerStay2DEvent(collider2D =>
            {
                HitHurtBox hitHurtBox = collider2D.GetComponent<HitHurtBox>();
                if (hitHurtBox != null)
                {
                    Player player = Player.Default;
                    if (hitHurtBox.Owner.CompareTag("Player"))
                    {
                        player.GravityEffect(this);
                    }
                }

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            GravityArea.OnTriggerExit2DEvent(collider2D =>
            {
                HitHurtBox hitHurtBox = collider2D.GetComponent<HitHurtBox>();
                if (hitHurtBox != null)
                {
                    Player player = Player.Default;
                    if (hitHurtBox.Owner.CompareTag("Player"))
                    {
                        player.GravityEffect(null);
                    }
                }

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            // 禁止开火区域
            HoldFireArea.OnTriggerEnter2DEvent(collider2D =>
            {
                HitHurtBox hitHurtBox = collider2D.GetComponent<HitHurtBox>();
                if (hitHurtBox != null)
                {
                    Player player = Player.Default;
                    if (hitHurtBox.Owner.CompareTag("Player"))
                    {
                        // 开启引导线
                        player.LineRenderer1.Show();
                        player.CanAttack = false;
                        // 提示
                        Text description = GamePanel.Default.SmallTitleText;
                        description.text = "开启路径预测\n请勿撞向星球";
                        description.Show().Delay(3f, () =>
                        {
                            description.Hide();
                        }).Execute();

                        // 关闭生成垃圾
                        Global.CanGenerate.Value = false;
                    }
                }

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            HoldFireArea.OnTriggerExit2DEvent(collider2D =>
            {
                HitHurtBox hitHurtBox = collider2D.GetComponent<HitHurtBox>();
                if (hitHurtBox != null)
                {
                    Player player = Player.Default;
                    if (hitHurtBox.Owner.CompareTag("Player"))
                    {
                        player.LineRenderer1.Hide();
                        player.CanAttack = true;

                        Text description = GamePanel.Default.SmallTitleText;
                        description.text = "关闭路径预测\n允许开火";
                        description.Show().Delay(3f, () =>
                        {
                            description.Hide();
                        }).Execute();

                        // 关闭生成垃圾
                        Global.CanGenerate.Value = true;
                    }
                }

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            // 自动防御/到达区域
            DefenseArea.OnTriggerEnter2DEvent(collider2D =>
            {
                HitHurtBox hitHurtBox = collider2D.GetComponent<HitHurtBox>();
                if (hitHurtBox != null)
                {
                    if (hitHurtBox.Owner.CompareTag("Player"))
                    {
                        // 提示到达该星球
                        Text description = GamePanel.Default.SmallTitleText;
                        description.text = "已到达！";
                        description.Show().Delay(3f, () =>
                        {
                            description.Hide();
                        }).Execute();

                        // 补充燃料
                        Global.Fuel.Value = Global.MaxFuel.Value;
                    }
                }

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            float stayTime = 0;

            DefenseArea.OnTriggerStay2DEvent(collider2D =>
            {
                HitHurtBox hitHurtBox = collider2D.GetComponent<HitHurtBox>();
                if (hitHurtBox != null)
                {
                    if (hitHurtBox.Owner.CompareTag("Player"))
                    {
                        stayTime += Time.deltaTime;

                        if (stayTime > 10f)
                        {
                            //TODO 获得成就
                        }
                    }

                    if (hitHurtBox.Owner.CompareTag("Asteroid"))
                    {
                        hitHurtBox.Owner.DestroySelfGracefully();
                    }
                }

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            DefenseArea.OnTriggerExit2DEvent(collider2D =>
            {
                HitHurtBox hitHurtBox = collider2D.GetComponent<HitHurtBox>();
                if (hitHurtBox != null)
                {
                    if (hitHurtBox.Owner.CompareTag("Player"))
                    {
                        // 短时间离开该星球，提示引力弹弓
                        if (stayTime <= 2f)
                        {
                            Text description = GamePanel.Default.SmallTitleText;
                            description.text = "引力弹弓！";
                            description.Show().Delay(3f, () =>
                            {
                                description.Hide();
                            }).Execute();
                        }
                    }
                }

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void Update()
        {
            if (IsRotate)
            {
                transform.Rotate(0, 0, mRandomRotationSpeed);
            }
        }
    }
}
