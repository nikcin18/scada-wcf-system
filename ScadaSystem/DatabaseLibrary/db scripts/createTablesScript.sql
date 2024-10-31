CREATE TABLE IF NOT EXISTS limits(
	limit_id VARCHAR(20) UNIQUE NOT NULL,
	min_val DECIMAL NOT NULL,
	max_val DECIMAL NOT NULL,
	PRIMARY KEY (limit_id),
	CONSTRAINT val_check CHECK (min_val<max_val)
);

CREATE TABLE IF NOT EXISTS sensors(
	sensor_id VARCHAR(20) UNIQUE NOT NULL,
	sensor_name VARCHAR(20) UNIQUE NOT NULL,
	active BOOLEAN DEFAULT FALSE,
	limit_id VARCHAR(20) REFERENCES limits(limit_id),
	last_measurement_id VARCHAR(20),
	PRIMARY KEY (sensor_id),
	CONSTRAINT name_check CHECK (
		LENGTH(sensor_name)<=15 AND 
		sensor_name ~ '[a-zA-Z0-9_\-]+')
);

CREATE TABLE IF NOT EXISTS measurements(
	measurement_id VARCHAR(20) UNIQUE NOT NULL,
	measurement_unit VARCHAR(5) NOT NULL,
	date_time TIMESTAMP,
	measured_value DECIMAL,
	sensor_id VARCHAR(20) REFERENCES sensors(sensor_id),
	PRIMARY KEY (measurement_id)
);

CREATE TABLE IF NOT EXISTS alarms(
	alarm_id VARCHAR(20) UNIQUE NOT NULL,
	sensor_id VARCHAR(20) REFERENCES sensors(sensor_id),
	alarm_type VARCHAR(10),
	date_time TIMESTAMP,
	PRIMARY KEY (alarm_id)
);