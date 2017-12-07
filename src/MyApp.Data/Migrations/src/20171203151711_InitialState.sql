CREATE TABLE IF NOT EXISTS pizzas (
  id UUID PRIMARY KEY NOT NULL,
  name TEXT NOT NULL,
  description TEXT NOT NULL,
  basePrice NUMERIC NOT NULL
);

CREATE TABLE IF NOT EXISTS toppings (
  id UUID PRIMARY KEY NOT NULL,
  name TEXT NOT NULL,
  description TEXT NOT NULL,
  price NUMERIC NOT NULL
);

CREATE TABLE IF NOT EXISTS pizza_toppings (
  pizza UUID REFERENCES pizzas (id),
  topping UUID REFERENCES toppings (id)
);


  