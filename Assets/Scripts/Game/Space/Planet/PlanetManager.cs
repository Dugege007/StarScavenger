using UnityEngine;
using QFramework;
using System.Collections.Generic;
using System.Linq;

namespace StarScavenger
{
    public partial class PlanetManager : ViewController
    {
        public List<Planet> Planets = new List<Planet>();

        private void Update()
        {
            if (Time.frameCount % 60 == 0 && Player.Default != null)
            {
                Vector3 playerPosition = Player.Default.transform.position;

                // ��ѯ���ҵ�����������������
                Planet nearestPlanet = Planets
                    .OrderBy(planet => (planet.transform.position - playerPosition).sqrMagnitude)
                    .FirstOrDefault();

                // ����Ϊ��һ��Ŀ������
                Player.Default.NextTargetPlanet = nearestPlanet;
            }
        }
    }
}