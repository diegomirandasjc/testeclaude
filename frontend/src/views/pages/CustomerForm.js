import React, { useState, useEffect } from "react";
import {
  Card,
  CardHeader,
  CardBody,
  Container,
  Row,
  Col,
  Button,
  Form,
  FormGroup,
  Input,
  Label,
  Alert
} from "reactstrap";
import { useNavigate, useParams, useLocation } from "react-router-dom";
import SimpleHeader from "components/Headers/SimpleHeader.js";
import api from "services/api";

const CustomerForm = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const location = useLocation();
  const isEditing = !!id;
  const isViewMode = location.pathname.split('/').pop() !== 'edit';

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [cities, setCities] = useState([]);
  const [formData, setFormData] = useState({
    name: "",
    cpf: "",
    cityId: ""
  });
  const [formErrors, setFormErrors] = useState({});

  const loadCustomer = async () => {
    if (!id) return;

    try {
      setLoading(true);
      const response = await api.get(`/api/customers/${id}`);
      setFormData({
        name: response.data.name,
        cpf: response.data.cpf,
        cityId: response.data.cityId
      });
    } catch (err) {
      console.error("Erro ao carregar cliente:", err);
      setError("Erro ao carregar dados do cliente");
    } finally {
      setLoading(false);
    }
  };

  const loadCities = async () => {
    try {
      const response = await api.get("/api/cities");
      if (response.data?.items) {
        setCities(response.data.items);
      }
    } catch (err) {
      console.error("Erro ao carregar cidades:", err);
      setError("Erro ao carregar cidades");
    }
  };

  useEffect(() => {
    loadCities();
    if (isEditing) {
      loadCustomer();
    }
  }, [id]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setFormErrors({});
    setLoading(true);

    try {
      if (isEditing) {
        await api.put(`/api/customers/${id}`, formData);
      } else {
        await api.post("/api/customers", formData);
      }
      navigate("/admin/customers");
    } catch (err) {
      console.error("Erro ao salvar cliente:", err);
      if (err.response?.data) {
        setFormErrors(err.response.data);
      } else {
        setError("Erro ao salvar cliente");
      }
    } finally {
      setLoading(false);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
    if (formErrors[name]) {
      setFormErrors(prev => ({
        ...prev,
        [name]: null
      }));
    }
  };

  if (loading && isEditing) {
    return (
      <>
        <SimpleHeader name="Cliente" parentName="Gestão" />
        <Container className="mt--6" fluid>
          <Card>
            <CardBody className="text-center">
              Carregando...
            </CardBody>
          </Card>
        </Container>
      </>
    );
  }

  return (
    <>
      <SimpleHeader name="Cliente" parentName="Gestão" />
      <Container className="mt--6" fluid>
        <Row>
          <Col>
            <Card>
              <CardHeader>
                <h3 className="mb-0">
                  {isViewMode ? "Visualizar Cliente" : (isEditing ? "Editar Cliente" : "Novo Cliente")}
                </h3>
              </CardHeader>
              <CardBody>
                {error && (
                  <Alert color="danger" toggle={() => setError(null)}>
                    {error}
                  </Alert>
                )}

                <Form onSubmit={handleSubmit}>
                  <Row>
                    <Col md="6">
                      <FormGroup>
                        <Label for="name">Nome</Label>
                        <Input
                          id="name"
                          name="name"
                          value={formData.name}
                          onChange={handleInputChange}
                          invalid={!!formErrors.Name}
                          disabled={isViewMode}
                        />
                        {formErrors.Name && (
                          <div className="invalid-feedback d-block">
                            {formErrors.Name.join(", ")}
                          </div>
                        )}
                      </FormGroup>
                    </Col>
                    <Col md="6">
                      <FormGroup>
                        <Label for="cpf">CPF</Label>
                        <Input
                          id="cpf"
                          name="cpf"
                          value={formData.cpf}
                          onChange={handleInputChange}
                          invalid={!!formErrors.Cpf}
                          disabled={isViewMode}
                        />
                        {formErrors.Cpf && (
                          <div className="invalid-feedback d-block">
                            {formErrors.Cpf.join(", ")}
                          </div>
                        )}
                      </FormGroup>
                    </Col>
                  </Row>
                  <Row>
                    <Col md="6">
                      <FormGroup>
                        <Label for="cityId">Cidade</Label>
                        <Input
                          type="select"
                          id="cityId"
                          name="cityId"
                          value={formData.cityId}
                          onChange={handleInputChange}
                          invalid={!!formErrors.CityId}
                          disabled={isViewMode}
                        >
                          <option value="">Selecione uma cidade</option>
                          {cities.map((city) => (
                            <option key={city.id} value={city.id}>
                              {city.name}
                            </option>
                          ))}
                        </Input>
                        {formErrors.CityId && (
                          <div className="invalid-feedback d-block">
                            {formErrors.CityId.join(", ")}
                          </div>
                        )}
                      </FormGroup>
                    </Col>
                  </Row>
                  <Button color="secondary" onClick={() => navigate("/admin/customers")} className="mr-2">
                    Voltar
                  </Button>
                  {!isViewMode && (
                    <Button color="primary" type="submit" disabled={loading}>
                      {loading ? "Salvando..." : "Salvar"}
                    </Button>
                  )}
                </Form>
              </CardBody>
            </Card>
          </Col>
        </Row>
      </Container>
    </>
  );
};

export default CustomerForm; 