// Core game logic for Meta Quest Tower Defense
AFRAME.registerComponent('game-manager', {
  schema: {
    maxHealth: {type: 'number', default: 100},
    currentHealth: {type: 'number', default: 100}
  },

  init: function () {
    console.log("Game Manager Initialized");
    this.coreEl = document.getElementById('core');
    this.coreLight = this.coreEl.querySelector('a-light');

    this.level = 1;
    this.paths = []; // Store path entity references
    this.pathPoints = []; // Store array of points for each path

    // Bind methods
    this.damageCore = this.damageCore.bind(this);
    this.updateCoreAppearance = this.updateCoreAppearance.bind(this);
    this.generatePaths = this.generatePaths.bind(this);
    this.updatePathColor = this.updatePathColor.bind(this);

    // Initial path generation
    this.generatePaths(this.level);

    // Listen for core health changes to update path colors
    this.el.addEventListener('core-health-changed', this.updatePathColor);

    // Enemy management
    this.enemies = [];
    this.spawnTimer = 0;
    this.spawnInterval = 2000; // ms
    this.enemiesSpawned = 0;
    this.enemiesPerLevel = 10;

    // Tower management
    this.towers = [];
    this.placeTower = this.placeTower.bind(this);

    // Listen for click/trigger to place tower
    document.querySelector('a-scene').addEventListener('click', this.placeTower);
  },

  placeTower: function (event) {
    // Only place on the grid/placeable area
    let intersection = event.detail.intersection;
    if (!intersection) return;

    // Basic cost/cooldown could be added here
    let point = intersection.point;

    let towerEl = document.createElement('a-entity');

    // Tron-like tower design (Energy pillar)
    towerEl.setAttribute('geometry', 'primitive: cylinder; radius: 0.2; height: 1.5');
    towerEl.setAttribute('material', 'color: #000000; metalness: 0.8; roughness: 0.2');
    towerEl.setAttribute('position', `${point.x} 0.75 ${point.z}`);

    // Energy ring
    let ringEl = document.createElement('a-entity');
    ringEl.setAttribute('geometry', 'primitive: torus; radius: 0.3; radiusTubular: 0.02');
    ringEl.setAttribute('material', 'color: #00ff00; emissive: #00ff00; emissiveIntensity: 2');
    ringEl.setAttribute('position', '0 0.5 0');
    ringEl.setAttribute('animation', 'property: rotation; to: 0 360 0; loop: true; dur: 3000; easing: linear');
    towerEl.appendChild(ringEl);

    this.el.sceneEl.appendChild(towerEl);

    this.towers.push({
      el: towerEl,
      x: point.x,
      z: point.z,
      range: 5,
      fireTimer: 0,
      fireRate: 1000 // ms
    });
  },

  tick: function (time, timeDelta) {
    if (this.data.currentHealth <= 0) return; // Don't tick if game over

    // Spawn enemies
    this.spawnTimer += timeDelta;
    if (this.spawnTimer > this.spawnInterval) {
      this.spawnTimer = 0;
      this.spawnEnemy();

      this.enemiesSpawned++;
      // Level progression logic
      if (this.enemiesSpawned >= this.enemiesPerLevel) {
        this.levelUp();
      }
    }

    // Update enemy positions
    for (let i = this.enemies.length - 1; i >= 0; i--) {
      let enemy = this.enemies[i];
      let pos = enemy.el.getAttribute('position');
      let target = enemy.targetPos;

      let dx = target.x - pos.x;
      let dz = target.z - pos.z;
      let dist = Math.sqrt(dx*dx + dz*dz);

      if (dist < 0.5) {
        // Reached core
        this.damageCore(10);
        enemy.el.parentNode.removeChild(enemy.el);
        this.enemies.splice(i, 1);
      } else {
        // Move towards core
        let speed = 0.05 * (timeDelta / 16.6); // Base speed
        let moveX = (dx / dist) * speed;
        let moveZ = (dz / dist) * speed;

        enemy.el.setAttribute('position', `${pos.x + moveX} ${pos.y} ${pos.z + moveZ}`);
      }
    }

    // Update towers
    this.towers.forEach(tower => {
      tower.fireTimer += timeDelta;
      if (tower.fireTimer > tower.fireRate) {
        // Find target
        for (let i = 0; i < this.enemies.length; i++) {
          let enemy = this.enemies[i];
          let ePos = enemy.el.getAttribute('position');
          let dx = ePos.x - tower.x;
          let dz = ePos.z - tower.z;
          let dist = Math.sqrt(dx*dx + dz*dz);

          if (dist < tower.range) {
            // Shoot! Create a laser
            this.shootLaser(tower, enemy);
            tower.fireTimer = 0;

            // Damage enemy
            enemy.health -= 10;
            if (enemy.health <= 0) {
              enemy.el.parentNode.removeChild(enemy.el);
              this.enemies.splice(i, 1);
            }
            break; // Only shoot one enemy per tick
          }
        }
      }
    });
  },

  shootLaser: function (tower, enemy) {
    let laserEl = document.createElement('a-entity');
    let ePos = enemy.el.getAttribute('position');

    // Draw a line from tower top to enemy
    laserEl.setAttribute('line', `start: ${tower.x} 1.5 ${tower.z}; end: ${ePos.x} ${ePos.y} ${ePos.z}; color: #00ff00; opacity: 1`);
    this.el.sceneEl.appendChild(laserEl);

    // Remove after a short duration
    setTimeout(() => {
      if (laserEl.parentNode) laserEl.parentNode.removeChild(laserEl);
    }, 100);
  },

  spawnEnemy: function () {
    if (this.pathPoints.length === 0) return;

    // Pick a random path
    const pathIndex = Math.floor(Math.random() * this.pathPoints.length);
    const path = this.pathPoints[pathIndex];

    let enemyEl = document.createElement('a-entity');

    // Make them look like corrupted bugs (glowing red/purple)
    enemyEl.setAttribute('geometry', 'primitive: dodecahedron; radius: 0.3');
    enemyEl.setAttribute('material', 'color: #ff0044; emissive: #ff0044; emissiveIntensity: 1; wireframe: true');
    enemyEl.setAttribute('position', `${path.start.x} 0.3 ${path.start.z}`);

    // Add small inner core to bug
    let innerBug = document.createElement('a-sphere');
    innerBug.setAttribute('radius', '0.15');
    innerBug.setAttribute('material', 'color: #8800ff; emissive: #8800ff; emissiveIntensity: 2');
    enemyEl.appendChild(innerBug);

    // Animation
    enemyEl.setAttribute('animation', 'property: rotation; to: 360 360 360; loop: true; dur: 2000; easing: linear');

    this.el.sceneEl.appendChild(enemyEl);

    this.enemies.push({
      el: enemyEl,
      targetPos: path.end,
      health: 20 + (this.level * 2) // Enemies get slightly stronger
    });
  },

  levelUp: function() {
    this.level++;
    this.enemiesSpawned = 0;
    this.enemiesPerLevel = 10 + (this.level * 5); // Require more enemies to progress next time
    this.spawnInterval = Math.max(500, this.spawnInterval * 0.9); // Spawn faster

    console.log(`Level Up! Now level ${this.level}`);

    // Check if we need to generate new paths
    if (this.level === 11 || this.level === 21) {
      this.generatePaths(this.level);
      // Immediately apply current core health colors to new paths
      this.updateCoreAppearance();
    }
  },

  generatePaths: function (level) {
    // Clear existing paths
    this.paths.forEach(p => p.parentNode.removeChild(p));
    this.paths = [];
    this.pathPoints = [];

    // Determine number of paths based on level
    let numPaths = 1;
    if (level >= 11 && level <= 20) numPaths = 2;
    if (level >= 21) numPaths = 3;

    // Radius from where enemies spawn
    const spawnRadius = 15;

    for (let i = 0; i < numPaths; i++) {
      // Create a path using A-Frame's line component for visual representation
      // We'll create a simple straight line for now, but this could be expanded to curve
      const angle = (Math.PI * 2 / numPaths) * i;
      const startX = Math.cos(angle) * spawnRadius;
      const startZ = Math.sin(angle) * spawnRadius;

      const startPoint = {x: startX, y: 0.1, z: startZ};
      const endPoint = {x: 0, y: 0.1, z: 0}; // Core

      this.pathPoints.push({start: startPoint, end: endPoint});

      let pathEl = document.createElement('a-entity');
      // Use a thick plane to represent the lit up path
      const length = Math.sqrt(startX*startX + startZ*startZ);
      pathEl.setAttribute('geometry', `primitive: plane; width: 0.5; height: ${length}`);

      // Calculate rotation to face core
      const rotY = -(Math.atan2(startX, startZ) * 180 / Math.PI);
      pathEl.setAttribute('rotation', `-90 ${rotY} 0`);

      // Position halfway between start and core
      pathEl.setAttribute('position', `${startX/2} 0.05 ${startZ/2}`);

      // Emissive material that matches core health
      let healthPercent = this.data.currentHealth / this.data.maxHealth;
      pathEl.setAttribute('material', `color: #00ffff; emissive: #00ffff; emissiveIntensity: ${healthPercent}; transparent: true; opacity: 0.6`);

      this.el.sceneEl.appendChild(pathEl);
      this.paths.push(pathEl);
    }
  },

  updatePathColor: function (event) {
    let healthPercent = event.detail.healthPercent;
    this.paths.forEach(pathEl => {
      pathEl.setAttribute('material', 'emissiveIntensity', healthPercent);
      pathEl.setAttribute('material', 'opacity', 0.2 + (0.4 * healthPercent));
    });
  },

  damageCore: function (amount) {
    this.data.currentHealth = Math.max(0, this.data.currentHealth - amount);
    this.updateCoreAppearance();

    if (this.data.currentHealth <= 0) {
      console.log("Game Over!");
      // Despawn all existing enemies
      this.enemies.forEach(enemy => {
        if (enemy.el.parentNode) {
          enemy.el.parentNode.removeChild(enemy.el);
        }
      });
      this.enemies = [];
    }
  },

  updateCoreAppearance: function () {
    let healthPercent = this.data.currentHealth / this.data.maxHealth;
    // Dim the core based on health
    this.coreEl.setAttribute('material', 'emissiveIntensity', healthPercent);
    this.coreEl.setAttribute('material', 'opacity', 0.5 + (0.5 * healthPercent));

    if (this.coreLight) {
      this.coreLight.setAttribute('intensity', 2 * healthPercent);
    }

    // Emit event so other systems (like paths) can update
    this.el.emit('core-health-changed', { healthPercent: healthPercent });
  }
});
