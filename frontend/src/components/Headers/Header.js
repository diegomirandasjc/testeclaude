import React from "react";
import { Container, Row, Col } from "reactstrap";

const Header = () => {
  return (
    <>
      <div className="header bg-gradient-info pb-8 pt-5 pt-md-8">
        <Container fluid>
          <div className="header-body">
            <Row>
              <Col lg="6" xl="3">
                <h6 className="text-uppercase text-light ls-1 mb-1">
                  Sistema de Gestão
                </h6>
              </Col>
            </Row>
          </div>
        </Container>
      </div>
    </>
  );
};

export default Header; 