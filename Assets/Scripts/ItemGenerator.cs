using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    public GameObject starPrefab;
    public GameObject superStarPrefab;
    public GameObject fuelPrefab;
    public GameObject rockPrefab;
    public FlightController flightController;
    public AudioSource starCollect;
    public AudioSource fuelCollect;
    private StarGenerator starGenerator;

    public int itemsGenerateAngle = 3;
    public float minHeight = 5;
    public float maxHeight = 45;

    private void Start()
    {
        starGenerator = new StarGenerator(flightController, starPrefab, superStarPrefab, starCollect);
    }

    private void FixedUpdate()
    {
        if (!flightController.isAlive) { return; }
        starGenerator.generateStars();
        if (Random.value > 0.98f)
        {
            float height = Random.Range(minHeight, maxHeight);
            float angle = flightController.aroundMapAngle + itemsGenerateAngle * Mathf.Deg2Rad;
            Vector3 spawnPosition = flightController.calculatePosition(height, angle, flightController.flightRadius);

            GameObject newObject;
            AudioSource audioSource = null;
            bool destroy = true;

            if (Random.value < 0.3f)
            {
                newObject = Instantiate(rockPrefab, spawnPosition, rockPrefab.transform.rotation);
                destroy = false;
            }
            else if (Random.value < 0.7f)
            {
                GameObject prefab = Random.value < 0.85f ? starPrefab : superStarPrefab;
                newObject = Instantiate(prefab, spawnPosition, prefab.gameObject.transform.rotation);
                audioSource = starCollect;
            }
            else
            {
                newObject = Instantiate(fuelPrefab, spawnPosition, fuelPrefab.gameObject.transform.rotation);
                audioSource = fuelCollect;
            }

            ItemController controller = newObject.GetComponent<ItemController>();
            controller.flightController = flightController;
            controller.height = height;
            controller.aroundMapAngle = angle;
            controller.destroyOnCollision = destroy;
            controller.audioSource = audioSource;
        }
    }


    class StarGenerator
    {
        private FlightController flightController;
        private GameObject starPrefab;
        private GameObject superStarPrefab;
        private AudioSource starCollect;
        private int howManyStarsGenerate;
        private float generationDistanseMeters = 100;
        private float betweenStarsDistanceMeters = 4;

        private float whenNextStarRadians;
        private float generationDistanseRadians;
        private float betweenStarDistanceRadians;

        private int arraysSize = 4;
        private int[] waveParams;
        private float sum;
        private float range = 10;


        public StarGenerator(FlightController flightController, GameObject starPrefab, GameObject superStarPrefab, AudioSource starCollect)
        {
            this.flightController = flightController;
            this.starPrefab = starPrefab;
            this.superStarPrefab = superStarPrefab;
            this.starCollect = starCollect;
            waveParams = new int[arraysSize];
            sum = 0;
            for (int i = 0; i < arraysSize; i++)
            {
                waveParams[i] = Random.Range(1, 11);
                sum += waveParams[i];
            }

            generationDistanseRadians = flightController.metersToRadians(generationDistanseMeters);
            betweenStarDistanceRadians = flightController.metersToRadians(betweenStarsDistanceMeters);
            updateNextStarsParameters();
        }

        private float getWave(float arg)
        {
            float x = arg / 5;
            float numerator = 0;
            for (int i = 0; i < arraysSize; i++)
            {
                numerator += waveParams[i] * Mathf.Sin(x / (i + 1));
            }
            return (numerator / sum * range) + range;
        }

        public void generateStars()
        {
            if (flightController.aroundMapAngle >= whenNextStarRadians)
            {
                float firstStarDistance = flightController.radiansToMeters(flightController.aroundMapAngle + generationDistanseRadians);
                for (int i = 0; i < howManyStarsGenerate; i++)
                {
                    float currentStarDistance = firstStarDistance + i * betweenStarsDistanceMeters;

                    float height = getWave(currentStarDistance);
                    float angle = flightController.metersToRadians(currentStarDistance);
                    Vector3 spawnPosition = flightController.calculatePosition(height, angle, flightController.flightRadius);

                    GameObject prefab = Random.value < 0.85f ? starPrefab : superStarPrefab;
                    GameObject newObject = Instantiate(prefab, spawnPosition, prefab.gameObject.transform.rotation);

                    ItemController controller = newObject.GetComponent<ItemController>();
                    controller.flightController = flightController;
                    controller.height = height;
                    controller.aroundMapAngle = angle;
                    controller.audioSource = starCollect;
                }
                updateNextStarsParameters();
            }
        }

        private void updateNextStarsParameters()
        {
            howManyStarsGenerate = Random.Range(3, 11);
            whenNextStarRadians = flightController.metersToRadians(Random.Range(40, 81)) + flightController.aroundMapAngle;
        }
    }

}