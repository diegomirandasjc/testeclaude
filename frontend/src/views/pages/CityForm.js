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

const CityForm = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const location = useLocation();
  const isEditing = !!id;
  const isViewMode = location.pathname.split('/').pop() !== 'edit';

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [formData, setFormData] = useState({
    name: ""
  });
  const [formErrors, setFormErrors] = useState({});

  const loadCity = async () => {
    if (!id) return;

    try {
      setLoading(true);
      const response = await api.get(`/api/cities/${id}`);
      setFormData({
        name: response.data.name
      });
    } catch (err) {
      console.error("Erro ao carregar cidade:", err);
      setError("Erro ao carregar dados da cidade");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (isEditing) {
      loadCity();
    }
  }, [id]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setFormErrors({});
    setLoading(true);

    try {
      if (isEditing) {
        await api.put(`/api/cities/${id}`, formData);
      } else {
        await api.post("/api/cities", formData);
      }
      navigate("/admin/cities");
    } catch (err) {
      console.error("Erro ao salvar cidade:", err);
      if (err.response?.data) {
        setFormErrors(err.response.data);
      } else {
        setError("Erro ao salvar cidade");
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
        <SimpleHeader name="Cidade" parentName="Gestão" />
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
      <SimpleHeader name="Cidade" parentName="Gestão" />
      <Container className="mt--6" fluid>
        <Row>
          <Col>
            <Card>
              <CardHeader>
                <h3 className="mb-0">
                  {isViewMode ? "Visualizar Cidade" : (isEditing ? "Editar Cidade" : "Nova Cidade")}
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
                  </Row>
                  <Button color="secondary" onClick={() => navigate("/admin/cities")} className="mr-2">
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

export default CityForm; 